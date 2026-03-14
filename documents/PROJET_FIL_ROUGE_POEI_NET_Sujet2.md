# TP — Plateforme de Trading Crypto Virtuel
## Architecture Microservices · ASP.NET Core 8 · Blazor

## 1. Contexte et Objectifs

Vous allez concevoir et développer **CryptoSim** — une plateforme de trading de cryptomonnaies fictives en environnement simulé. L'objectif est de mettre en pratique l'architecture microservices dans un contexte réaliste : chaque service est indépendant, possède sa propre base de données, et communique via HTTP REST ou SignalR.

L'application doit permettre de :

- Créer un compte et s'authentifier via JWT
- Consulter les prix de cryptomonnaies fictives mis à jour en temps réel
- Passer des ordres d'achat et de vente
- Gérer un portefeuille virtuel (solde, actifs, performance)
- Visualiser l'historique des transactions et des cours

> **Contrainte architecturale**
>
> L'application est décomposée en **4 microservices indépendants** + **1 frontend Blazor**.
> Chaque service possède sa propre base de données (*pattern Database per Service*).
> La sécurité est assurée par des **tokens JWT** émis par l'`AuthService` et validés par chaque service.
> Les prix de marché sont poussés en temps réel via **SignalR**.

---

## 2. Architecture Générale

### 2.1 Vue d'ensemble des services

| Service | Port | Responsabilité | Base de données |
|---|---|---|---|
| `AuthService` | 5001 | Inscription, connexion, émission JWT | MySql (`users_db`) |
| `MarketService` | 5002 | Génération des prix, historique des cours | MySql (`market_db`) |
| `PortfolioService` | 5003 | Portefeuille, transactions, performance | MySql (`portfolio_db`) |
| `OrderService` | 5004 | Passage et suivi des ordres d'achat/vente | MySql (`orders_db`) |
| `Blazor Frontend` | 5000 | Interface utilisateur | — |

### 2.2 Schéma de communication

```
                        ┌─────────────────────────────────────┐
                        │         Blazor Frontend (5000)       │
                        └────┬────────┬────────┬────────┬─────┘
                             │ HTTP   │ HTTP   │ HTTP   │ HTTP
                             ▼        ▼        ▼        ▼
                    ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
                    │   Auth   │ │  Market  │ │Portfolio │ │  Order   │
                    │ Service  │ │ Service  │ │ Service  │ │ Service  │
                    │  :5001   │ │  :5002   │ │  :5003   │ │  :5004   │
                    └──────────┘ └──────────┘ └──────────┘ └──────────┘
                         │            │             │            │
                       users_db   market_db   portfolio_db   orders_db
```

### 2.3 Flux d'authentification JWT

1. Le client envoie `POST /api/auth/login` avec `{ username, password }`
2. L'`AuthService` valide les credentials et retourne un JWT signé (HMAC-SHA256)
3. Le client stocke le token et l'inclut dans chaque requête : `Authorization: Bearer <token>`
4. Chaque microservice valide le token **localement** via le middleware `JwtBearer`
5. Les endpoints protégés retournent `401` si le token est absent, invalide ou expiré

### 2.4 Structure de la solution

```
CryptoSim.sln
├── AuthService/
│   ├── Controllers/AuthController.cs
│   ├── Models/User.cs
│   ├── DTOs/
│   ├── Services/JwtService.cs
│   ├── Data/AuthDbContext.cs
│   └── appsettings.json
├── MarketService/
│   ├── Controllers/MarketController.cs
│   ├── Models/
│   ├── Services/PriceSimulatorService.cs
│   └── Data/MarketDbContext.cs
├── PortfolioService/
│   ├── Controllers/PortfolioController.cs
│   ├── Models/
│   ├── Services/
│   └── Data/PortfolioDbContext.cs
├── OrderService/
│   ├── Controllers/OrderController.cs
│   ├── Models/
│   ├── Services/
│   └── Data/OrderDbContext.cs
├── CryptoSim.Blazor/
│   ├── Pages/
│   ├── Services/           ← HttpClient wrappers
│   ├── Components/
│   └── appsettings.json
└── docker-compose.yml
```

---

## 3. Modélisation

### 3.1 Diagramme de classes UML

> 📋 **Travail demandé**
>
> Produisez un diagramme de classes complet pour chaque microservice.
> Indiquez clairement à quel service appartient chaque classe.
> Représentez les dépendances entre services par des relations de type **association par ID** (pas de navigation EF entre services).

Classes attendues par service (liste non exhaustive) :

**AuthService :** `User`, `Role` (enum), `JwtService`, `RegisterRequest`, `LoginRequest`, `AuthResponse`

**MarketService :** `Crypto`, `PriceSnapshot`, `PriceHistory`, `PriceSimulatorService`, `MarketHub`

**PortfolioService :** `Portfolio`, `Holding`, `Transaction`, `PortfolioSummary`

**OrderService :** `Order`, `OrderStatus` (enum), `OrderType` (enum), `OrderRequest`, `OrderResponse`

### 3.2 MLD (Modèle Logique de Données)

> 📋 **Travail demandé**
>
> Produisez le MLD complet de chaque base de données.
> Rappel : **pas de clé étrangère entre les bases de différents services**.

**AuthService — `users_db` :**
```
Users(Id PK, Username VARCHAR(50) UNIQUE NOT NULL, PasswordHash TEXT NOT NULL,
      Email VARCHAR(255) UNIQUE NOT NULL, Role VARCHAR(20) NOT NULL DEFAULT 'User',
      Balance DECIMAL(18,8) NOT NULL DEFAULT 10000, CreatedAt DATETIME NOT NULL)
```

> Chaque nouvel utilisateur reçoit un **solde virtuel de départ de 10 000 $** pour trader.

**MarketService — `market_db` :**
```
Cryptos(Id PK, Symbol VARCHAR(10) UNIQUE NOT NULL, Name VARCHAR(100) NOT NULL,
        CurrentPrice DECIMAL(18,8) NOT NULL, LastUpdated DATETIME NOT NULL)

PriceHistory(Id PK, CryptoSymbol VARCHAR(10) NOT NULL, Price DECIMAL(18,8) NOT NULL,
             RecordedAt DATETIME NOT NULL)
```

**PortfolioService — `portfolio_db` :**
```
Holdings(Id PK, UserId INT NOT NULL, CryptoSymbol VARCHAR(10) NOT NULL,
         Quantity DECIMAL(18,8) NOT NULL, AverageBuyPrice DECIMAL(18,8) NOT NULL,
         UpdatedAt DATETIME NOT NULL)

Transactions(Id PK, UserId INT NOT NULL, CryptoSymbol VARCHAR(10) NOT NULL,
             Type VARCHAR(10) NOT NULL, Quantity DECIMAL(18,8) NOT NULL,
             PriceAtTime DECIMAL(18,8) NOT NULL, Total DECIMAL(18,8) NOT NULL,
             ExecutedAt DATETIME NOT NULL)
```

**OrderService — `orders_db` :**
```
Orders(Id PK, UserId INT NOT NULL, CryptoSymbol VARCHAR(10) NOT NULL,
       Type VARCHAR(10) NOT NULL, Quantity DECIMAL(18,8) NOT NULL,
       Price DECIMAL(18,8) NOT NULL, Status VARCHAR(20) NOT NULL,
       CreatedAt DATETIME NOT NULL, ExecutedAt DATETIME)
```

---

## 4. Microservice : AuthService (port 5001)

### 4.1 Modèle `User`

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.User;
    public decimal Balance { get; set; } = 10_000m; // solde virtuel en $
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum Role { User, Admin }
```

### 4.2 DTOs

```csharp
public record RegisterRequest(
    [Required][MinLength(3)] string Username,
    [Required][MinLength(6)] string Password,
    [Required][EmailAddress] string Email
);

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

public record AuthResponse(
    string Token,
    string Username,
    string Role,
    decimal Balance,
    long ExpiresIn
);
```

### 4.3 Endpoints REST

| Méthode | URL | Auth | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` | Non | Créer un compte (solde initial : 10 000 $) |
| `POST` | `/api/auth/login` | Non | Connexion, retourne un JWT |
| `GET` | `/api/auth/me` | Oui | Profil + solde de l'utilisateur connecté |
| `GET` | `/api/auth/balance` | Oui | Solde disponible uniquement |

### 4.4 Logique métier

- Le mot de passe est haché avec **BCrypt** avant stockage (`BCrypt.Net.BCrypt.HashPassword()`)
- Le JWT contient les claims : `sub` (userId), `name` (username), `role`
- Le token expire après **24h**
- À l'inscription, le solde virtuel est initialisé à **10 000 $**

### 4.5 Configuration JWT (`appsettings.json`)

```json
{
  "Jwt": {
    "Secret": "<clé-256bits-base64>",
    "Issuer": "CryptoSim.AuthService",
    "Audience": "CryptoSim.Services",
    "ExpirationMinutes": 1440
  }
}
```

---

## 5. Microservice : MarketService (port 5002)

### 5.1 Cryptomonnaies fictives

Le service gère les cryptomonnaies suivantes (données de départ à insérer via seed) :

| Symbol | Nom | Prix initial |
|---|---|---|
| `BTC-X` | BitcoinX | 42 000 $ |
| `ETH-Z` | EtherZero | 2 500 $ |
| `SOL-F` | SolaFake | 95 $ |
| `DOG-M` | DogeMoon | 0.08 $ |
| `ADA-S` | CardanoSim | 0.45 $ |

### 5.2 Modèles

```csharp
public class Crypto
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class PriceHistory
{
    public int Id { get; set; }
    public string CryptoSymbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime RecordedAt { get; set; }
}
```

### 5.3 Simulation des prix

Le service génère des variations de prix de manière automatique via un **`BackgroundService`** :

```csharp
public class PriceSimulatorService : BackgroundService
{
    // Toutes les 3 secondes :
    // 1. Appliquer une variation aléatoire entre -2% et +2% à chaque crypto
    // 2. Mettre à jour CurrentPrice en base
    // 3. Insérer un enregistrement dans PriceHistory
    // 4. Diffuser les nouveaux prix via SignalR (MarketHub)
}
```

**Règle de variation :**
```csharp
var variation = (decimal)(Random.Shared.NextDouble() * 4 - 2) / 100; // -2% à +2%
crypto.CurrentPrice = Math.Max(0.01m, crypto.CurrentPrice * (1 + variation));
```

### 5.4 Hub SignalR

```csharp
public class MarketHub : Hub
{
    // Méthode appelée par le serveur pour diffuser les prix
    // Client-side : await hubConnection.On<List<PriceUpdate>>("ReceivePrices", ...)
}
```

Les clients Blazor se connectent au hub et reçoivent les mises à jour en temps réel sans polling.

### 5.5 Endpoints REST

| Méthode | URL | Auth | Description |
|---|---|---|---|
| `GET` | `/api/market/cryptos` | Non | Liste des cryptos avec prix actuel |
| `GET` | `/api/market/cryptos/{symbol}` | Non | Détails d'une crypto |
| `GET` | `/api/market/history/{symbol}?limit=50` | Non | Historique des prix (limité) |
| `GET` | `/api/market/snapshot` | Non | Snapshot complet du marché |

---

## 6. Microservice : OrderService (port 5004)

### 6.1 Modèles

```csharp
public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CryptoSymbol { get; set; } = string.Empty;
    public OrderType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }           // prix au moment de l'ordre
    public decimal Total { get; set; }           // Quantity * Price
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExecutedAt { get; set; }
}

public enum OrderType   { Buy, Sell }
public enum OrderStatus { Pending, Executed, Cancelled, Rejected }
```

### 6.2 DTOs

```csharp
public record OrderRequest(
    [Required] string CryptoSymbol,
    [Required] OrderType Type,
    [Range(0.00000001, double.MaxValue)] decimal Quantity
);

public record OrderResponse(
    int OrderId,
    string CryptoSymbol,
    OrderType Type,
    decimal Quantity,
    decimal Price,
    decimal Total,
    OrderStatus Status,
    DateTime ExecutedAt
);
```

### 6.3 Logique métier

Lors du traitement d'un ordre d'**achat** :

1. Appeler `GET /api/market/cryptos/{symbol}` pour récupérer le prix actuel
2. Calculer le total : `Total = Quantity × Price`
3. Vérifier que l'utilisateur a un solde suffisant (appel à `AuthService`)
4. Si OK : créer l'ordre en base avec `Status = Executed`, déduire le solde, créer le holding
5. Si KO : créer l'ordre avec `Status = Rejected`, retourner `400`

Lors du traitement d'un ordre de **vente** :

1. Vérifier que l'utilisateur possède la quantité demandée (appel à `PortfolioService`)
2. Calculer le total : `Total = Quantity × PrixActuel`
3. Si OK : exécuter la vente, créditer le solde, mettre à jour le holding
4. Si KO : retourner `400` avec message d'erreur explicite

### 6.4 Endpoints REST

| Méthode | URL | Auth | Description |
|---|---|---|---|
| `POST` | `/api/orders` | Oui | Passer un ordre d'achat ou de vente |
| `GET` | `/api/orders` | Oui | Historique des ordres de l'utilisateur connecté |
| `GET` | `/api/orders/{id}` | Oui | Détails d'un ordre |
| `DELETE` | `/api/orders/{id}` | Oui | Annuler un ordre en attente |

---

## 7. Microservice : PortfolioService (port 5003)

### 7.1 Modèles

```csharp
public class Holding
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CryptoSymbol { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal AverageBuyPrice { get; set; }  // prix moyen d'achat
    public DateTime UpdatedAt { get; set; }
}

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CryptoSymbol { get; set; } = string.Empty;
    public OrderType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal PriceAtTime { get; set; }
    public decimal Total { get; set; }
    public DateTime ExecutedAt { get; set; }
}
```

### 7.2 Calcul des gains/pertes non réalisés

Pour chaque holding, le gain/perte non réalisé est calculé à la volée :

```
GainLoss = (PrixActuel - AverageBuyPrice) × Quantity
GainLossPercent = ((PrixActuel - AverageBuyPrice) / AverageBuyPrice) × 100
```

> Le `PrixActuel` est récupéré depuis `MarketService` au moment de la requête.

### 7.3 DTO `PortfolioSummary`

```csharp
public record HoldingDetail(
    string Symbol,
    string Name,
    decimal Quantity,
    decimal AverageBuyPrice,
    decimal CurrentPrice,
    decimal CurrentValue,
    decimal GainLoss,
    decimal GainLossPercent
);

public record PortfolioSummary(
    int UserId,
    decimal CashBalance,
    decimal TotalInvested,
    decimal TotalCurrentValue,
    decimal TotalGainLoss,
    decimal TotalGainLossPercent,
    List<HoldingDetail> Holdings
);
```

### 7.4 Endpoints REST

| Méthode | URL | Auth | Description |
|---|---|---|---|
| `GET` | `/api/portfolio` | Oui | Résumé complet du portefeuille |
| `GET` | `/api/portfolio/holdings` | Oui | Liste des actifs détenus |
| `GET` | `/api/portfolio/transactions` | Oui | Historique des transactions |
| `GET` | `/api/portfolio/performance` | Oui | Performance globale (gain/perte total) |

---

## 8. Frontend Blazor (port 5000)

### 8.1 Pages à implémenter

| Page | Route | Auth | Description |
|---|---|---|---|
| Accueil / Marché | `/` | Non | Tableau des prix en temps réel via SignalR |
| Inscription | `/register` | Non | Formulaire de création de compte |
| Connexion | `/login` | Non | Formulaire de login, stockage du JWT |
| Portefeuille | `/portfolio` | Oui | Résumé, holdings, gains/pertes |
| Trading | `/trade/{symbol}` | Oui | Formulaire d'achat/vente, graphique des prix |
| Historique | `/history` | Oui | Transactions et ordres passés |

## 9. Contraintes Techniques

### Packages NuGet requis par service

| Package | AuthService | MarketService | OrderService | PortfolioService |
|---|---|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | ✅ | ✅ | ✅ | ✅ |
| `Microsoft.EntityFrameworkCore.Sqlite` | ✅ | ✅ | ✅ | ✅ |   <-- mysql pas sqlite
| `Microsoft.EntityFrameworkCore.Tools` | ✅ | ✅ | ✅ | ✅ |
| `BCrypt.Net-Next` | ✅ | ❌ | ❌ | ❌ |
| `System.IdentityModel.Tokens.Jwt` | ✅ | ❌ | ❌ | ❌ |
| `Microsoft.AspNetCore.OpenApi` + Scalar/Swagger | ✅ | ✅ | ✅ | ✅ |

### Rappels importants

- Le **secret JWT doit être identique** dans tous les services (variable d'environnement)
- Tous les endpoints sauf `/api/auth/login` et `/api/auth/register` sont protégés par `[Authorize]`
- Les services qui appellent d'autres services utilisent `IHttpClientFactory` (pas de `new HttpClient()`)
- Utilisez `app.UseCors()` pour autoriser les appels depuis le frontend Blazor
- Chaque service expose **Swagger/Scalar** en développement (`app.MapOpenApi()`)

---

## 10. Livrables

### 10.1 Documentation (Bonus)

- **Diagramme de classes UML** — un par service, avec attributs et méthodes principales
- **MLD complet** — un par base de données, avec types SQL et contraintes
- **Diagramme d'architecture** — vue globale des services et de leurs interactions
- **Documentation API** — export Swagger JSON ou Markdown des endpoints

### 11.2 Code source

- Code source complet de chaque microservice
- Code source du frontend Blazor
- Fichier `docker-compose.yml` fonctionnel
- Fichier `.env.example` avec les variables d'environnement nécessaires
- `README.md` avec les instructions de lancement

### 11.3 Démonstration

Lors de la présentation, vous devrez démontrer :

1. L'inscription et la connexion d'un utilisateur
2. L'affichage des prix en temps réel (variation visible à l'écran)
3. Le passage d'un ordre d'achat
4. L'affichage du portefeuille avec gain/perte
5. Le passage d'un ordre de vente et le crédit du solde

---

>  **Bonus**
>
> - Tests unitaires xUnit sur la logique de simulation de prix et le calcul gain/perte
> - Graphique d'évolution des prix dans l'interface Blazor (Chart.js ou Blazor.ApexCharts)
> - Implémentation d'un système d'ordres limites (ordre exécuté quand le prix atteint un seuil)

---
