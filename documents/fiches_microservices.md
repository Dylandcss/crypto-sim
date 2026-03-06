### Fiche AuthService

| Caractéristique | Description |
| :--- | :--- |
| **Nom** | `AuthService` |
| **Port** | `5001` |
| **Base de données** | `users_db` |
| **Rôle** | <ul><li><strong>Inscription :</strong> Créer un nouvel utilisateur en base.</li><li><strong>Initialisation :</strong> Attribuer le solde de départ (10 000 €) à la création.</li><li><strong>Authentification :</strong> Vérifier le couple login/mot de passe.</li><li><strong>Sécurité :</strong> Générer les Tokens JWT pour les sessions.</li><li><strong>Consultation :</strong> Fournir le solde actuel de l'utilisateur.</li><li><strong>Opération financière :</strong> Mettre à jour le solde (créditer ou débiter des dollars) suite à un ordre validé.</li></ul> |
| **De quels autres services a-t-il besoin ?** | **Aucun.** Il est autonome. |
| **Qui va l'utiliser ?** | <ul><li><strong>Frontend (React) :</strong> Pour l'inscription, le login et l'affichage du solde.</li><li><strong>OrderService :</strong> Pour vérifier si l'utilisateur a assez d'argent avant d'acheter, et pour débiter le compte.</li><li><strong>PortfolioService :</strong> Pour récupérer le solde afin de calculer la valeur totale du portefeuille.</li></ul> |
| **Points d'attention** | <ul><li><strong>Hachage :</strong> Stockage des mots de passe (BCrypt).</li><li><strong>Endpoint manquant :</strong> L'énoncé ne précise pas l'URL pour modifier le solde depuis l'extérieur (<code>OrderService</code>), il faudra la créer (ex: <code>PUT /api/auth/balance</code>).</li><li><strong>Architecture :</strong> Ce n'est pas son role de gérer le solde de l'utilisateur.</li></ul> |

---

### Fiche MarketService

| Caractéristique | Description |
| :--- | :--- |
| **Nom** | `MarketService` |
| **Port** | `5002` |
| **Base de données** | `market_db` |
| **Rôle** | <ul><li><strong>Gestion du référentiel :</strong> Stocker la liste des cryptos disponibles (BTC, ETH, etc.).</li><li><strong>Simulation de marché (Cœur du service) :</strong> Un `BackgroundService` tourne en boucle (toutes les 3s) pour faire varier les prix aléatoirement (-2% à +2%).</li><li><strong>Historisation :</strong> Sauvegarder chaque variation de prix pour créer des graphiques.</li><li><strong>Diffusion Temps Réel :</strong> Pousser les nouveaux prix aux clients connectés via **SignalR** (WebSockets).</li><li><strong>API de consultation :</strong> Fournir le prix "à l'instant T" via REST.</li></ul> |
| **De quels autres services a-t-il besoin ?** | **Aucun.** Il est autonome. |
| **Qui va l'utiliser ?** | <ul><li><strong>Frontend (React) :</strong> Se connecte pour afficher les courbes et les prix en direct.</li><li><strong>OrderService :</strong> Il appelle ce service pour demander : <em>"Combien coûte <strong>1 unité de cette crypto</strong> (ex: BTC) maintenant ?"</em> afin de calculer le total de la commande.</li><li><strong>PortfolioService :</strong> Il appelle ce service pour demander les prix actuels de <strong>toutes les cryptos</strong> afin de calculer la valeur totale du portefeuille de l'utilisateur.</li></ul> |
| **Points d'attention** | <ul><li><strong>Performance :</strong> Le `BackgroundService` ne doit pas bloquer l'application.</li><li><strong>Volume de données :</strong> La table `PriceHistory` va grossir très vite (5 cryptos * 1 entrée toutes les 3s).</li><li><strong>CORS :</strong> SignalR est capricieux avec les origines croisées, il faudra bien configurer le CORS pour React.</li></ul> |
| **Spécificité : Le BackgroundService** | C'est un processus (Thread) qui tourne en arrière-plan **en continu**.<br>Toutes les **3 secondes**, il :<br>1. Calcule une variation aléatoire (-2% à +2%) pour chaque crypto.<br>2. Met à jour la base de données (`CurrentPrice` et `History`).<br>3. **Pousse** les nouveaux prix vers le Frontend via SignalR. |
---


### Fiche PortfolioService

| Caractéristique | Description |
| :--- | :--- |
| **Nom** | `PortfolioService` |
| **Port** | `5003` |
| **Base de données** | `portfolio_db` |
| **Rôle** | <ul><li><strong>Gestion des actifs :</strong> Suivre précisément combien de crypto (BTC, ETH, etc.) chaque utilisateur possède.</li><li><strong>Calcul de performance :</strong> Calculer le gain ou la perte "non réalisé" (la différence entre le prix d'achat initial et le prix actuel du marché).</li><li><strong>Historique transactionnel :</strong> Garder une trace de chaque mouvement (achat ou vente).</li><li><strong>Synthèse :</strong> Fournir une vue globale (total investi, solde de cash, valeur actuelle).</li></ul> |
| **Dépendances (Il appelle qui ?)** | <ul><li><strong>MarketService :</strong> Il doit appeler l'API `GET /api/market/cryptos` pour obtenir les prix actuels nécessaires au calcul des gains/pertes.</li><li><strong>AuthService :</strong> Il doit appeler l'API `GET /api/auth/balance` pour obtenir le solde de dollars (cash) et compléter le résumé financier.</li></ul> |
| **Qui l'utilise ?** | <ul><li><strong>Frontend (React) :</strong> Pour afficher la page "Portefeuille" (résumé financier, liste des cryptos, graphique de performance).</li><li><strong>OrderService :</strong> Il consulte ce service pour vérifier si un utilisateur possède assez de cryptos avant de valider un ordre de <strong>Vente</strong>.</li></ul> |
| **Points d'attention** | <ul><li><strong>Calculs à la volée :</strong> La valeur du portefeuille change toutes les 3 secondes (puisque le marché bouge). Le calcul doit être fait en temps réel à chaque requête de l'utilisateur.</li><li><strong>Endpoints internes :</strong> Comme pour l'AuthService, il faudra créer des endpoints internes (ex: `POST /api/portfolio/add-holding`) pour que l'OrderService puisse mettre à jour le portefeuille après une transaction.</li></ul> |

---

### Fiche OrderService

| Caractéristique | Description |
| :--- | :--- |
| **Nom** | `OrderService` |
| **Port** | `5004` |
| **Base de données** | `orders_db` |
| **Rôle** | <ul><li><strong>Réception d'ordres :</strong> Recevoir et valider les demandes d'achat ou de vente de l'utilisateur (via React).</li><li><strong>Orchestration (Logique métier) :</strong> C'est le cerveau qui vérifie les conditions avant d'autoriser une transaction.</li><li><strong>Suivi des ordres :</strong> Stocker tous les ordres passés (qu'ils soient `Pending`, `Executed` ou `Rejected`).</li></ul> |
| **Dépendances (Il appelle qui ?)** | <ul><li><strong>AuthService :</strong> Il l'appelle pour vérifier le solde (achat) et pour lui demander de mettre à jour le solde (créditer/débiter).</li><li><strong>MarketService :</strong> Il l'appelle pour obtenir le prix unitaire actuel d'une crypto afin de calculer le montant total de l'ordre.</li><li><strong>PortfolioService :</strong> Il l'appelle pour vérifier si l'utilisateur possède bien la crypto (vente) et pour lui demander de mettre à jour son inventaire (Ajout/Retrait de Holding + Historique transactionnel).</li></ul> |
| **Qui l'utilise ?** | <ul><li><strong>Frontend (React) :</strong> Pour envoyer le formulaire de commande et consulter l'historique des ordres passés.</li></ul> |
| **Points d'attention** | <ul><li><strong>Gestion des transactions :</strong> C'est le point critique. Si l'achat est validé en base `orders_db` mais que l'appel au `PortfolioService` échoue, tu te retrouves avec une incohérence (un ordre "validé" mais pas de crypto ajoutée).</li><li><strong>Sécurité :</strong> Les endpoints de création d'ordres doivent être protégés par JWT.</li><li><strong>Validation :</strong> Toujours valider les données envoyées par l'utilisateur (ex: quantité > 0).</li></ul> |

---

### Fiche API Gateway

| Caractéristique | Description |
| :--- | :--- |
| **Nom** | `Gateway` |
| **Port** | `5000` |
| **Base de données** | **Aucune.** Elle ne stocke rien. |
| **Rôle** | <ul><li><strong>Centralisation :</strong> Toutes les requêtes du frontend arrivent ici.</li><li><strong>Routage :</strong> Redirige automatiquement vers le bon microservice (ex: `/auth/*` → `5001`, `/market/*` → `5002`).</li><li><strong>Sécurité :</strong> Peut vérifier la validité du JWT une seule fois pour tout le monde.</li><li><strong>Masquage :</strong> Cache les ports réels des microservices au monde extérieur.</li></ul> |
| **De quels autres services a-t-il besoin ?** | **Aucun.** |
| **Qui va l'utiliser ?** | <ul><li><strong>Frontend (React) :</strong> Il ne connaît que l'URL de la Gateway (ex: `localhost:5000/api/...`). Il n'a plus besoin de connaître les ports 5001, 5002, 5003, 5004.</li></ul> |
| **Points d'attention** | <ul><li><strong>CORS :</strong> C'est ici qu'on va centraliser la configuration CORS au lieu de le faire dans chaque microservice.</li></ul> |


