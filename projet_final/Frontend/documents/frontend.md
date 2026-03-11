# Front End — CryptoSim

---

## 📦 Dépendances à installer

| Package | Rôle |
| --- | --- |
| `react-router-dom` | Navigation entre pages |
| `@microsoft/signalr` | Prix crypto en temps réel (hub SignalR backend sur `/markethub`) |
| `recharts` | Graphiques (historique des prix, répartition portefeuille) |

```bash
npm install react-router-dom @microsoft/signalr recharts
```

---

## 🔌 Services (couche API) — `src/services/`

| Fichier | Fonctions à implémenter | Endpoints Gateway |
| --- | --- | --- |
| `authService.js` | `login`, `register`, `getProfile`, `getBalance` | `POST /api/auth/login`, `POST /api/auth/register`, `GET /api/auth/me`, `GET /api/auth/balance` |
| `marketService.js` | `getAllCryptos`, `getCrypto(symbol)`, `getPriceHistory(symbol, limit)`, `getSnapshot(date)`, connexion **SignalR** | `GET /api/market`, `GET /api/market/{symbol}`, `GET /api/market/history/{symbol}`, `GET /api/market/snapshot` |
| `orderService.js` | `createOrder(dto)`, `getOrders()`, `getOrder(id)`, `deleteOrder(id)` | `POST /api/orders`, `GET /api/orders`, `GET /api/orders/{id}`, `DELETE /api/orders/{id}` |
| `portfolioService.js` | `getSummary()`, `getHoldings()`, `getHolding(symbol)`, `getTransactions()`, `getPerformance()` | `GET /api/portfolio`, `GET /api/portfolio/holdings`, `GET /api/portfolio/holdings/{symbol}`, `GET /api/portfolio/transactions`, `GET /api/portfolio/performance` |

---

## 🧩 Composants à développer

### 1. Contexte & Auth — `src/context/`

| Composant | Rôle |
| --- | --- |
| `AuthContext.js` | Stocker le token JWT, l'utilisateur connecté, les fonctions `login`/`logout`/`register`. Fournir un hook `useAuth()`. Centraliser la gestion du token (actuellement directement dans `App.js` via `localStorage`). |

---

### 2. Layout & Navigation — `src/components/`

| Composant | Rôle |
| --- | --- |
| `Navbar.js` | Barre de navigation entre les pages (Marché, Portefeuille, Ordres, Profil). Afficher le username + solde cash. Bouton déconnexion. |
| `PrivateRoute.js` | Wrapper pour protéger les routes authentifiées (redirige vers `/login` si pas de token). |

---

### 3. Pages d'authentification — `src/pages/`

| Composant | Rôle |
| --- | --- |
| `LoginPage.js` | Formulaire login → JWT → `localStorage`. |
| `RegisterPage.js` | Formulaire d'inscription : `username`, `email`, `password`. Appelle `POST /api/auth/register`. Redirige vers login après succès. |

---

### 4. Page Marché — `src/pages/Marche.js` + composants

| Composant | Rôle |
| --- | --- |
| `Marche.js` (page) | Page principale du marché. Liste toutes les cryptos avec prix temps réel via SignalR. |
| `CryptoList.js` | Tableau/grille des cryptos (Symbol, Nom, Prix actuel, variation). Écoute le hub SignalR `ReceivePrices` pour mise à jour live. |
| `CryptoCard.js` | Carte individuelle par crypto affichant symbol, nom, prix, variation. Clic → détail. |
| `CryptoDetail.js` | Vue détaillée d'une crypto : prix actuel + graphique historique (`GET /api/market/history/{symbol}`). Boutons Acheter/Vendre (nécessite d'être authentifié). |
| `PriceChart.js` | Graphique d'historique des prix (line chart avec `recharts`). Données de `GET /api/market/history/{symbol}?limit=50`. |

---

### 5. Page Ordres — `src/pages/Ordres.js` + composants

| Composant | Rôle |
| --- | --- |
| `Ordres.js` (page) | Page listant tous les ordres de l'utilisateur. |
| `OrderForm.js` | Formulaire de passage d'ordre : choix crypto (symbol), type (Buy/Sell), quantité. Appelle `POST /api/orders`. |
| `OrderList.js` | Tableau des ordres passés : symbol, type, quantité, prix, total, statut (Pending/Executed/Rejected), date. |
| `OrderCard.js` | Ligne/carte individuelle d'un ordre avec statut coloré. Option de suppression si `Pending`. |

---

### 6. Page Portefeuille — `src/pages/Portefeuille.js` + composants

| Composant | Rôle |
| --- | --- |
| `Portefeuille.js` (page) | Dashboard principal du portefeuille. |
| `PortfolioSummary.js` | Résumé global : solde cash, valeur totale des holdings, gain/perte total (P&L). Données de `GET /api/portfolio`. |
| `HoldingsList.js` | Liste des cryptos détenues : symbol, quantité, prix moyen d'achat, prix actuel, gain/perte par holding. Données de `GET /api/portfolio/holdings`. |
| `HoldingDetail.js` | Vue détaillée d'un holding spécifique (`GET /api/portfolio/holdings/{symbol}`). |
| `TransactionHistory.js` | Historique des transactions (achat/vente) avec date, type, quantité, prix. Données de `GET /api/portfolio/transactions`. |
| `PortfolioChart.js` | Graphique de répartition du portefeuille. |
| `PerformanceCard.js` | Carte affichant la performance globale (gain/perte en $ et %). Données de `GET /api/portfolio/performance`. |

---

### 7. Page Profil

| Composant | Rôle |
| --- | --- |
| `ProfilPage.js` | Afficher les infos utilisateur (`GET /api/auth/me`) : username, email, rôle, solde, date d'inscription. |

---

### 8. Composants utilitaires — `src/components/`

| Composant | Rôle |
| --- | --- |
| `Button.js` | Bouton réutilisable. |
| `LoadingSpinner.js` | Indicateur de chargement pendant les appels API. |
| `ErrorMessage.js` | Affichage standardisé des erreurs (le backend renvoie des exceptions typées). |

---

## 🗺️ Structure de routing

| Route | Page | Auth requise |
| --- | --- | --- |
| `/login` | `LoginPage` | ❌ |
| `/register` | `RegisterPage` | ❌ |
| `/` ou `/market` | `Marche` | ❌ |
| `/market/:symbol` | `CryptoDetail` | ❌ |
| `/portfolio` | `Portefeuille` | ✅ |
| `/orders` | `Ordres` | ✅ |
| `/profile` | `ProfilPage` | ✅ |

---

## 🔄 Flux clés

### Temps réel (SignalR)
Le `MarketSimulatorService` backend met à jour les prix toutes les **3 secondes** et broadcast via **SignalR** → le frontend doit se connecter au hub `/markethub` pour recevoir l'événement `ReceivePrices` (tableau de `{ symbol, name, currentPrice, updatedAt }`).

### Passage d'ordre
`OrderForm` → `POST /api/orders` avec `{ symbol, type (Buy/Sell), quantity }` → le backend orchestre tout (vérification solde, débit, mise à jour portefeuille) → rafraîchir le portefeuille et la liste d'ordres côté frontend.

### Authentification
JWT stocké dans `localStorage`, envoyé dans le header `Authorization: Bearer {token}` pour toutes les routes protégées.

---

## 📊 Récapitulatif

| Catégorie | Nombre de composants |
| --- | --- |
| Context | 1 |
| Layout & Navigation | 2 |
| Pages Auth | 2 |
| Page Marché | 5 |
| Page Ordres | 4 |
| Page Portefeuille | 7 |
| Page Profil | 1 |
| Utilitaires | 3 |
| **Total** | **~25 composants** |