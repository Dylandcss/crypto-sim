# Ordres Limites — Comment ça marche

## C'est quoi un ordre limite ?

Un ordre "au marché" s'exécute immédiatement au prix actuel.
Un ordre "limite" s'exécute plus tard, **quand le prix atteint un seuil que tu fixes** :

- **Achat limite** : "Achète-moi X BTC quand le prix descend à Y$"
- **Vente limite** : "Vends mes X BTC quand le prix monte à Y$"

---

## Étape 1 — Modèle de données

### Avant les ordres limites

Le modèle `Order` contenait uniquement les champs d'un ordre au marché : `Price`, `Total`, `Status`, etc.

### Migration ajoutée

```
AddLimitOrderFields (20260314202521)
```

Un seul champ nullable ajouté à la table `Orders` :

```csharp
public decimal? LimitPrice { get; set; }
```

C'est nullable exprès : si `LimitPrice` est `null`, c'est un ordre au marché. Si elle a une valeur, c'est un ordre limite. Un seul champ suffit pour distinguer les deux types.

---

## Étape 2 — Création de l'ordre (Controller → Service)

Quand le frontend appelle `POST /api/orders`, le `OrderRequest` peut maintenant contenir un `LimitPrice` optionnel.

Dans `OrderManagementService.CreateOrderAsync()`, la logique se divise dès le début :

```csharp
// Ordre limite : enregistrer comme Pending sans exécuter
if (request.LimitPrice.HasValue)
{
    var limitOrder = new Order
    {
        ...
        Price = request.LimitPrice.Value,
        Total = request.Quantity * request.LimitPrice.Value,
        LimitPrice = request.LimitPrice.Value,
        Status = OrderStatus.Pending,   // <-- jamais Executed tout de suite
    };
    return (await _repository.AddOrderAsync(limitOrder)).ToDto();
}
```

**Point clé** : l'ordre limite ne fait rien immédiatement. Il est juste sauvegardé en base avec le statut `Pending`. Aucun transfert d'argent, aucune mise à jour du portfolio à ce stade.

---

## Étape 3 — Le BackgroundService : LimitOrderExecutorService

C'est le cœur du système. Ce service tourne **en permanence en arrière-plan**, indépendamment des requêtes HTTP.

```csharp
public class LimitOrderExecutorService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await orderService.ExecuteLimitOrdersAsync();
            await Task.Delay(5000, stoppingToken);  // vérifie toutes les 5 secondes
        }
    }
}
```

**Problème de design à résoudre** : un `BackgroundService` tourne dans un scope Singleton (il vit toute la durée de l'app). Mais `IOrderService` est enregistré en `Scoped` (un par requête HTTP). On ne peut pas injecter un Scoped dans un Singleton directement.

**Solution** : on injecte `IServiceScopeFactory` et on crée un nouveau scope à chaque cycle :

```csharp
using var scope = _scopeFactory.CreateScope();
var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
await orderService.ExecuteLimitOrdersAsync();
```

---

## Étape 4 — Logique d'exécution des ordres limites

Dans `ExecuteLimitOrdersAsync()` :

```csharp
var pendingOrders = await _repository.GetPendingLimitOrdersAsync();

foreach (var order in pendingOrders)
{
    decimal currentPrice = await _marketClient.GetCryptoPriceAsync(order.CryptoSymbol, "");

    bool shouldExecute = order.Type == OrderType.Buy
        ? currentPrice <= order.LimitPrice!.Value   // achat : attendre que ça descende
        : currentPrice >= order.LimitPrice!.Value;  // vente : attendre que ça monte

    if (!shouldExecute) continue;

    order.Price = currentPrice;     // exécuté AU PRIX ACTUEL, pas au prix limite
    order.Total = order.Quantity * currentPrice;

    await ExecuteOrderCoreInternalAsync(order, currentPrice);
    await _repository.UpdateOrderStatusAsync(order.Id, OrderStatus.Executed, DateTime.UtcNow);
}
```

**Point important** : l'ordre est exécuté au **prix du marché au moment de l'exécution**, pas exactement au prix limite. C'est le comportement réel des bourses : le prix limite est un déclencheur, pas un prix garanti (en pratique sur un simulateur c'est souvent le même, mais la logique est correcte).

Si l'exécution échoue (solde insuffisant, erreur réseau...), l'exception est silencieusement ignorée et l'ordre reste `Pending` pour le prochain cycle.

---

## Étape 5 — Pourquoi on a besoin de la X-Internal-Key

### Le problème

Un ordre au marché est déclenché par l'utilisateur via HTTP. Sa requête contient un **JWT** dans le header `Authorization`. OrderService reçoit ce token, et quand il doit appeler AuthService ou PortfolioService, il le transmet : les autres services savent ainsi quel utilisateur est concerné et peuvent vérifier ses droits.

Un ordre limite est déclenché par `LimitOrderExecutorService`. Ce service tourne en arrière-plan, de façon autonome, **sans requête HTTP entrante et donc sans token JWT**. Il n'a aucun moyen de représenter un utilisateur précis via JWT.

### Les alternatives écartées

| Option | Problème |
|--------|----------|
| Stocker le JWT de l'utilisateur en base lors de la création de l'ordre | Les tokens JWT expirent — l'ordre serait bloqué si le token est périmé au moment de l'exécution |
| Créer un compte "service" avec son propre JWT admin | Sur-ingénierie, nécessite une gestion de droits supplémentaire |
| Appeler les endpoints publics sans auth | Impossible — les endpoints de modification de solde et de portfolio sont protégés par `[Authorize]` |

### La solution : clé partagée entre services

On définit un secret (`INTERNAL_API_KEY`) dans la configuration de chaque service (variable d'environnement / docker-compose). C'est une simple chaîne de caractères connue uniquement des microservices, jamais exposée au client.

Côté **réception** (AuthService, PortfolioService), chaque controller a une méthode privée :

```csharp
private bool IsInternalRequest() =>
    Request.Headers.TryGetValue("X-Internal-Key", out var key) &&
    key == _config[EnvConstants.InternalApiKey];
```

Les endpoints internes appellent `IsInternalRequest()` à la place de `[Authorize]` :

```csharp
// Endpoint accessible uniquement depuis un autre service
[HttpGet("internal/balance/{userId}")]
public async Task<IActionResult> GetUserBalance(int userId)
{
    if (!IsInternalRequest()) return Forbid();
    // ...
}
```

Côté **émission** (OrderService via `BaseApiClient`), la clé est injectée dans le header de la requête HTTP :

```csharp
if (!string.IsNullOrEmpty(internalKey))
    request.Headers.Add("X-Internal-Key", internalKey);
```

`BaseApiClient` expose deux familles de méthodes pour distinguer les deux contextes :

```csharp
// Appel standard avec JWT (ordres au marché, déclenchés par l'utilisateur)
protected Task<T> GetAsync<T>(string endpoint, string token)

// Appel interne avec la clé partagée (ordres limites, BackgroundService)
protected Task<T> GetInternalAsync<T>(string endpoint, string internalKey)
```

### Ce que ça garantit

- Les endpoints `/internal/...` ne sont **pas exposés via la Gateway** — ils ne sont joignables qu'en réseau interne Docker
- Même si quelqu'un connaissait l'URL, il lui faudrait aussi la valeur de `INTERNAL_API_KEY`
- OrderService n'a pas besoin du token de l'utilisateur : il passe juste son `UserId` (stocké dans l'ordre en base) pour que AuthService sache quel compte débiter

---

## Résumé du flux complet

```
Utilisateur                  OrderService              MarketService     AuthService    PortfolioService
    |                             |                         |                |               |
    |-- POST /orders (limitPrice) |                         |                |               |
    |                        save Pending                   |                |               |
    |<-- 200 (ordre enregistré)   |                         |                |               |
    |                             |                         |                |               |
    |          [toutes les 5s — BackgroundService]          |                |               |
    |                        GetPendingLimitOrders          |                |               |
    |                             |-- GET /market/{symbol} ->|               |               |
    |                             |<-- currentPrice ---------|               |               |
    |                        prix atteint ?                  |                |               |
    |                             |-- GetBalance (X-Internal-Key) ---------->|               |
    |                             |-- UpdateBalance (X-Internal-Key) ------->|               |
    |                             |-- UpdateHolding (X-Internal-Key) ---------------------->|
    |                        UpdateOrderStatus → Executed    |                |               |
```

---

## Ce que le frontend fait de son côté

`TradeForm` rafraîchit automatiquement le solde et les holdings **toutes les 15 secondes** via `setInterval`. C'est pour refléter l'exécution éventuelle d'un ordre limite côté serveur, sans que l'utilisateur ait à recharger la page.

```js
useEffect(() => {
    const interval = setInterval(() => {
        refreshUser();    // rafraîchit le solde
        fetchHolding();   // rafraîchit la quantité détenue
    }, 15000);
    return () => clearInterval(interval);  // cleanup obligatoire
}, [symbol]);
```

### Pas de risque de fuite mémoire

Le `return () => clearInterval(interval)` est la **fonction de cleanup** du `useEffect`. React l'appelle automatiquement dans deux situations :

- Quand le composant est **démonté** (l'utilisateur quitte la page Trade) → l'interval est stoppé
- Quand `symbol` **change** (l'utilisateur navigue vers une autre crypto) → l'ancien interval est stoppé, un nouveau est créé pour le nouveau symbole

Sans ce cleanup, chaque navigation créerait un nouvel interval sans supprimer l'ancien — les intervals s'accumuleraient et appelleraient l'API en parallèle. C'est le pattern classique de fuite mémoire React avec `setInterval`.
