# Structure des bases de données


### 1. `users_db` (AuthService)
**Rôle :** Gérer l'identité de l'utilisateur et son solde.

| Table | Champ | Type | Description |
| :--- | :--- | :--- | :--- |
| **Users** | `Id` | `INT` | Identifiant unique de l'utilisateur. |
| | `Username` | `VARCHAR` | Nom d'utilisateur (pseudo). |
| | `PasswordHash` | `TEXT` | Mot de passe sécurisé (haché). |
| | `Email` | `VARCHAR` | Adresse email de contact. |
| | `Role` | `VARCHAR` | Rôle de l'utilisateur (User/Admin). |
| | `Balance` | `DECIMAL` | Solde disponible en € |
| | `CreatedAt` | `DATETIME` | Date de création du compte. |

---
<br>

### 2. `market_db` (MarketService)
**Rôle :** Référencer les cryptos et garder un historique de leur prix.

| Table | Champ | Type | Description |
| :--- | :--- | :--- | :--- |
| **Cryptos** | `Id` | `INT` | Identifiant de la crypto. |
| | `Symbol` | `VARCHAR` | Symbole court (ex: BTC-X). |
| | `Name` | `VARCHAR` | Nom complet (ex: BitcoinX). |
| | `CurrentPrice`| `DECIMAL` | Prix actuel de 1 unité. |
| | `LastUpdated` | `DATETIME` | Date de la dernière mise à jour. |
| - |
| **PriceHistory**| `Id` | `INT` | Identifiant unique de la trace. |
| | `CryptoId` | `INT` | Référence à la crypto concernée (FK). |
| | `Price` | `DECIMAL` | Prix constaté à ce moment-là. |
| | `RecordedAt` | `DATETIME` | Heure précise de l'enregistrement. |

---
<br>

### 3. `portfolio_db` (PortfolioService)
**Rôle :** Suivre les cryptos possédées et l'historique des achats/ventes.

| Table | Champ | Type | Description |
| :--- | :--- | :--- | :--- |
| **Holdings** | `Id` | `INT` | Identifiant unique de la possession. |
| | `UserId` | `INT` | ID de l'utilisateur propriétaire. |
| | `CryptoSymbol` | `VARCHAR` | Symbole de la crypto détenue. |
| | `Quantity` | `DECIMAL` | Quantité totale actuelle détenue. |
| | `AvgBuyPrice` | `DECIMAL` | Prix moyen d'achat pour calculer la plus-value. |
| - |
| **Transactions** | `Id` | `INT` | Identifiant de la transaction. |
| | `HoldingId` | `INT` | Référence à la position concernée (FK). |
| | `Type` | `VARCHAR` | Type d'action (Achat ou Vente). |
| | `Quantity` | `DECIMAL` | Quantité achetée ou vendue. |
| | `Total` | `DECIMAL` | Montant total payé ou reçu (€). |
| | `ExecutedAt` | `DATETIME` | Date précise de l'exécution. |

---
<br>

### 4. `orders_db` (OrderService)
**Rôle :** Gérer le cycle de vie des ordres (demandes d'achat/vente).

| Table | Champ | Type | Description |
| :--- | :--- | :--- | :--- |
| **Orders** | `Id` | `INT` | Identifiant unique de l'ordre. |
| | `UserId` | `INT` | ID de l'utilisateur qui passe l'ordre. |
| | `CryptoSymbol` | `VARCHAR` | Symbole de la crypto ciblée. |
| | `Type` | `VARCHAR` | Achat ou Vente. |
| | `Quantity` | `DECIMAL` | Quantité voulue. |
| | `Price` | `DECIMAL` | Prix unitaire lors de l'ordre. |
| | `Total` | `DECIMAL` | Prix * Quantité (€). |
| | `Status` | `VARCHAR` | État (Pending, Executed, Rejected). |
| | `CreatedAt` | `DATETIME` | Date de la création de la demande. |

