# CryptoSim — Frontend React : Guide pas à pas

> **Objectif de ce document** : Retracer, étape par étape, la construction du frontend React de CryptoSim — comme si on venait de tout écrire. Chaque section explique *ce qu'on veut accomplir* et *à quoi réfléchir*, sans reproduire le code lui-même.

---

## Sommaire

1. [Initialisation du projet](#1-initialisation-du-projet)
2. [Système de design et variables CSS](#2-système-de-design-et-variables-css)
3. [Authentification](#3-authentification)
4. [Mise en place du routage](#4-mise-en-place-du-routage)
5. [Layout : Sidebar et Navbar](#5-layout--sidebar-et-navbar)
6. [Page Market — liste en temps réel](#6-page-market--liste-en-temps-réel)
7. [Page Portfolio — tableau de bord](#7-page-portfolio--tableau-de-bord)
8. [Page Trade — passer un ordre](#8-page-trade--passer-un-ordre)
9. [Page Historique](#9-page-historique)
10. [Page Profil](#10-page-profil)
11. [Composants utilitaires](#11-composants-utilitaires)
12. [Effet CRT](#12-effet-crt)
13. [Déploiement Docker](#13-déploiement-docker)

---

## 1. Initialisation du projet

### Objectif
Mettre en place un projet React moderne, rapide à compiler, prêt pour le développement.

### Sous-objectifs

**1.1 — Créer le projet avec Vite**

On choisit **Vite** plutôt que Create React App pour sa vitesse de démarrage et son HMR (hot module replacement) quasi-instantané. On utilise le template `react-swc` qui remplace Babel par le compilateur Rust SWC — significativement plus rapide.

À retenir : Vite sert les fichiers en ES modules natifs pendant le développement, ce qui évite de tout rebundler à chaque modification.

**1.2 — Installer les dépendances**

On identifie dès le départ les librairies dont on aura besoin :

- **react-router-dom v7** — navigation côté client (SPA)
- **@microsoft/signalr** — connexion WebSocket au hub de prix en temps réel
- **chart.js + react-chartjs-2** — graphiques de prix et d'allocation de portefeuille
- **@nsmr/pixelart-react** — icônes pixel art cohérentes avec le thème rétro

**1.3 — Organiser la structure des dossiers**

Dès le départ, on définit une architecture claire qui grandira proprement :

```
src/
├── assets/         ← polices, images, icônes de cryptos
├── components/     ← composants réutilisables (un dossier par composant)
├── context/        ← providers React Context (auth, CRT)
├── hooks/          ← hooks personnalisés
├── pages/          ← une page = une route = un dossier
├── services/       ← fonctions d'appel API, regroupées par domaine
├── styles/         ← variables CSS globales
└── utils/          ← fonctions utilitaires pures
```

L'idée : chaque composant vit dans son propre dossier avec son fichier `.jsx` et son `.module.css`. Ça évite les collisions de noms de classes et ça rend les imports explicites.

**1.4 — Configurer les variables d'environnement**

On a trois URLs à configurer :
- `VITE_API_BASE_URL` — base URL pour tous les appels REST via la Gateway
- `VITE_BASE_URL` — URL racine (pour les cas où on ne passe pas par `/api`)
- `VITE_SIGNALR_HUB_URL` — URL directe vers le hub SignalR de MarketService

Important : SignalR ne passe **pas** par la Gateway HTTP car les WebSockets nécessitent un proxy spécial (YARP côté backend). En développement, on pointe directement sur le port de MarketService.

On crée un `.env.example` pour documenter ces variables sans exposer de valeurs réelles dans le dépôt.

---

## 2. Système de design et variables CSS

### Objectif
Définir une charte visuelle cohérente, maintenable, avec un thème rétro/retro-futuriste.

### Sous-objectifs

**2.1 — Définir les variables CSS globales**

Tout ce qui se répète visuellement devient une variable CSS dans `variables.css`. On définit :

- **Palette de couleurs** : fond principal sombre (`#181B24`), fond sidebar (`#191434`), couleurs d'accent (bleu clair, vert clair, rouge clair, orange clair) pour les variations de prix et les statuts
- **Typographie** : police principale VT323 (monospace rétro), tailles de xs à xxl
- **Espacements** : une échelle de 4px → 8px → 16px → 24px → 32px → 64px
- **Effets** : ombres, rayons de bordure, transitions prédéfinies

L'avantage : changer la couleur d'accent dans toute l'app se fait en une ligne.

**2.2 — Choisir la police**

On utilise **VT323**, une police monospace qui imite les anciens terminaux. Elle est intégrée via Google Fonts dans `index.html` (pas en local pour éviter d'alourdir le bundle). On la déclare en fallback `monospace`.

**2.3 — Reset et styles globaux**

Dans `index.css`, on applique un reset minimal (box-sizing, margin/padding zéro sur body), on définit le fond et la couleur de texte par défaut, et on normalise les boutons pour ne pas avoir les styles natifs du navigateur.

---

## 3. Authentification

### Objectif
Permettre à l'utilisateur de se connecter et s'inscrire, et maintenir son état d'authentification dans toute l'application.

### Sous-objectifs

**3.1 — Créer le AuthContext**

C'est la pièce centrale. On crée un Context React qui stocke :
- Le **token JWT** de l'utilisateur
- Les **données utilisateur** (username, role, balance, etc.)
- Des **drapeaux d'état** : `isLoading`, `isAuthenticated`

Les méthodes exposées :
- `loginUser(token, user)` — persiste le token et les données dans le localStorage + met à jour l'état
- `logoutUser()` — nettoie le localStorage et réinitialise l'état
- `refreshUser()` — rappelle `GET /auth/me` pour rafraîchir les données (utile après un achat qui modifie le solde)

À la montée du composant, on lit le localStorage pour restaurer la session existante.

Cas important à gérer : les réponses 401 de l'API. Plutôt que de gérer ça dans chaque appel, on utilise un **event custom** : quand un appel reçoit 401, il dispatch un événement `'unauthorized'` sur `window`. Le AuthContext écoute cet événement et déclenche `logoutUser()` automatiquement.

**3.2 — Créer les services API auth**

Dans `authService.js`, on écrit les fonctions qui appellent les endpoints backend :
- Login : POST vers `/api/auth/login` avec `{ username, password }`
- Register : POST vers `/api/auth/register` avec `{ username, email, password }`
- GetMe : GET vers `/api/auth/me` avec le header `Authorization: Bearer <token>`

On crée aussi un wrapper `safeFetch()` dans `api.js` qui intercept les erreurs réseau et les transforme en messages lisibles (plutôt que de laisser passer des `TypeError: Failed to fetch`).

**3.3 — Formulaire de connexion**

Le `LoginForm` gère son propre état local (champs du formulaire, erreur d'affichage, état de chargement). La soumission :
1. Valide que les champs ne sont pas vides
2. Appelle `authService.login()`
3. En cas de succès, appelle `loginUser()` du contexte et redirige vers `/market`
4. En cas d'erreur, affiche le message via `DisplayMessage`

**3.4 — Formulaire d'inscription**

Similaire au login, avec un champ email supplémentaire. On valide le format email côté client. Après inscription réussie, on redirige vers la page de login (l'utilisateur n'est pas auto-connecté).

---

## 4. Mise en place du routage

### Objectif
Définir la navigation entre les pages et protéger les routes qui nécessitent d'être authentifié.

### Sous-objectifs

**4.1 — Définir les routes dans App.jsx**

On utilise `<Routes>` de React Router v7. Structure :
- Routes publiques : `/`, `/login`, `/register`
- Routes protégées : tout le reste, wrappées dans `<ProtectedRoute>`

**4.2 — Créer le ProtectedRoute**

Ce composant est la porte d'entrée de toute la partie authentifiée. Il vérifie `isAuthenticated` depuis AuthContext. Si non authentifié, il redirige vers `/login`. Sinon, il rend le layout applicatif (Sidebar + Navbar + `<Outlet />`).

C'est ici qu'on intègre le layout principal : chaque page protégée est automatiquement entourée de la Sidebar et de la Navbar, sans avoir à les répéter dans chaque page.

**4.3 — Navigation programmatique**

On utilise `useNavigate()` pour les redirections après action (login, logout, passage d'un ordre). On utilise `<NavLink>` dans la Sidebar pour bénéficier de l'état `isActive` automatique qui permet de styler le lien courant.

---

## 5. Layout : Sidebar et Navbar

### Objectif
Offrir une navigation persistante et un bandeau d'information en haut de l'écran sur toutes les pages authentifiées.

### Sous-objectifs

**5.1 — Sidebar**

La Sidebar est fixe à gauche. Elle contient :
- Le logo de l'application
- Les liens de navigation principaux avec une icône pixel art chacun : Market, Portfolio, Historique, Profil
- En bas : le bouton de toggle de l'effet CRT (avec icône TV)

Pour les liens actifs, `<NavLink>` fournit automatiquement la classe `active`. On l'utilise avec une fonction pour appliquer les styles de l'item actif.

**5.2 — Navbar**

La Navbar est en haut. Elle affiche :
- Le titre de la page courante (injecté depuis chaque page via une prop ou un contexte simple)
- Un message de bienvenue avec le username
- Le solde actuel avec une icône de sac d'argent
- Le bouton de déconnexion

Le solde est lu depuis `AuthContext`. Il se rafraîchit à intervalles réguliers (toutes les 15 secondes) via `refreshUser()` pour rester à jour sans avoir besoin d'une action de l'utilisateur.

**5.3 — Mise en page générale**

Le layout combine Sidebar (largeur fixe) + zone principale (flex-grow, scroll indépendant). La Navbar est dans la zone principale, au-dessus du contenu de la page.

---

## 6. Page Market — liste en temps réel

### Objectif
Afficher la liste des cryptomonnaies avec leurs prix qui se mettent à jour en temps réel via SignalR.

### Sous-objectifs

**6.1 — Créer le service MarketService**

Dans `marketService.js`, les fonctions d'appel API :
- `getSnapshot()` — charge tous les prix en une seule requête au chargement initial
- `getCryptos()` — liste des cryptos avec leurs métadonnées
- `getCryptoBySymbol(symbol)` — détails d'une crypto spécifique
- `getPriceHistory(symbol, limit, skip)` — historique de prix pour les graphiques
- `createSignalRConnection()` — configure et retourne une connexion SignalR

**6.2 — Créer le hook useSignalR**

Ce hook encapsule tout le cycle de vie d'une connexion SignalR :
1. Au montage : ouvre la connexion et s'abonne à l'événement `ReceivePrices`
2. En cours : reçoit les mises à jour de prix et appelle un callback fourni par le composant
3. Au démontage : ferme proprement la connexion

Il retourne `{ isLive, error }` pour que le composant puisse afficher l'état de la connexion (point vert/rouge).

La stratégie de reconnexion automatique est configurée avec un backoff exponentiel : 0ms, 2s, 5s, 10s, 30s. Si la connexion tombe, SignalR réessaie automatiquement.

**6.3 — Composant MarketItem**

Chaque carte de crypto affiche :
- L'icône de la crypto (depuis `coinIcons.js`)
- Le nom et le symbole
- Le prix actuel formaté
- Une flèche haut/bas colorée selon la direction du dernier mouvement de prix

Pour la direction, on compare le nouveau prix avec le précédent à chaque mise à jour SignalR. On stocke localement la direction (`up`/`down`/`neutral`) pour appliquer le style correspondant (vert/rouge).

**6.4 — Composant MarketGrid**

Grille responsive de `MarketItem`. Reçoit la liste des cryptos avec leurs prix en prop et distribue aux cards.

**6.5 — Page Market**

La page orchestre tout :
1. Au chargement : appelle `getSnapshot()` pour avoir les prix initiaux
2. S'abonne à SignalR via `useSignalR()`
3. À chaque mise à jour SignalR, met à jour les prix dans son état local
4. Fournit une barre de recherche qui filtre la liste par nom ou symbole
5. Rend `<MarketGrid>` avec les données filtrées

Chaque carte est cliquable et navigue vers `/trade/:symbol`.

---

## 7. Page Portfolio — tableau de bord

### Objectif
Donner à l'utilisateur une vue complète de son portefeuille : solde, positions, gains/pertes, répartition.

### Sous-objectifs

**7.1 — Créer le service PortfolioService**

Fonctions d'appel API :
- `getPortfolioSummary()` — résumé global (valeur totale, solde, gains/pertes)
- `getHoldings()` — liste des positions ouvertes
- `getHoldingBySymbol(symbol)` — détail d'une position
- `getTransactions()` — historique des transactions exécutées
- `getPerformance()` — métriques de performance (rentabilité, etc.)

Toutes ces fonctions passent le token JWT en header Authorization.

**7.2 — Composant PortfolioSummary**

Bloc de statistiques clés :
- Solde disponible en cash
- Valeur totale du portefeuille (cash + valeur des positions au prix actuel)
- Montant investi (somme des achats historiques)
- Gain/perte en valeur absolue et en pourcentage

Les gains/pertes sont colorés : vert si positif, rouge si négatif. On utilise la fonction utilitaire `gainLossColor()` pour ça.

**7.3 — Composant PortfolioChart**

Graphique en anneau (Doughnut) montrant la répartition du portefeuille par crypto. Chaque segment représente une position, dimensionné par sa valeur actuelle.

Configuration Chart.js :
- Pas de légende intégrée (on la gère manuellement pour le style)
- Tooltip personnalisé affichant le symbole, la valeur et le pourcentage
- Couleurs cohérentes avec le thème sombre

**7.4 — Composant HoldingsList**

Tableau des positions :
- Symbole + icône de la crypto
- Quantité détenue
- Prix moyen d'achat
- Prix actuel (mis à jour en temps réel via SignalR si connecté)
- Valeur actuelle de la position
- Gain/perte sur cette position
- Lien vers la page de détail de la position

**7.5 — Composant PerformanceCard**

Carte récapitulative des métriques de performance. Affiche les données retournées par l'endpoint `/portfolio/performance` du backend.

**7.6 — Page Portfolio**

Charge en parallèle les données de résumé, les positions, et la performance. Utilise le hook `useFetch` pour gérer les états de chargement et d'erreur de chaque bloc indépendamment.

**7.7 — Sous-page HoldingDetails**

Page accessible depuis HoldingsList via `/portfolio/holdings/:symbol`. Affiche le détail d'une position unique :
- Prix moyen d'achat vs prix actuel
- Quantité et valeur totale
- Gain/perte calculé

---

## 8. Page Trade — passer un ordre

### Objectif
Permettre à l'utilisateur d'acheter ou vendre une crypto, en ordre au marché ou à cours limité.

### Sous-objectifs

**8.1 — Créer le service OrderService**

Fonctions :
- `createOrder(dto)` — POST un nouvel ordre (market ou limit)
- `getOrders()` — liste des ordres de l'utilisateur
- `cancelOrder(orderId)` — annuler un ordre en attente

**8.2 — Composant PriceChart**

Graphique en courbe (Line) de l'historique des prix d'une crypto. L'axe X représente le temps (timestamps), l'axe Y le prix.

Points techniques :
- On utilise `getPriceHistory(symbol)` pour charger les données historiques
- On s'abonne à SignalR pour ajouter les nouveaux prix en temps réel (fenêtre glissante de 30 secondes)
- Les options Chart.js sont configurées pour un rendu fluide sur fond sombre (pas de grille lourde, couleurs douces)

**8.3 — Composant TradeForm**

Formulaire de passage d'ordre avec deux modes :

**Ordre au marché (Market Order)** :
- Champ : quantité à acheter/vendre
- Affichage du prix actuel (mis à jour en temps réel)
- Estimation du total en temps réel (`quantité × prix`)
- Validation : solde suffisant pour un achat, quantité détenue suffisante pour une vente

**Ordre à cours limité (Limit Order)** :
- Champs : quantité + prix limite
- L'ordre sera exécuté seulement quand le prix atteint le seuil
- Badge "En attente" dans l'interface pour indiquer que l'exécution n'est pas immédiate

L'onglet Achat/Vente change les couleurs du formulaire (vert pour achat, rouge pour vente) et adapte les validations.

**8.4 — Page Trade**

Affiche côte à côte `PriceChart` (gauche) et `TradeForm` (droite). Reçoit le symbol depuis les paramètres de route (`useParams()`). Charge les métadonnées de la crypto, le solde et la position existante au chargement.

Après un ordre réussi : appelle `refreshUser()` pour mettre à jour le solde affiché dans la Navbar.

---

## 9. Page Historique

### Objectif
Permettre à l'utilisateur de consulter l'historique de ses ordres et transactions, et d'annuler les ordres en attente.

### Sous-objectifs

**9.1 — Structure à onglets**

La page se divise en deux onglets :
- **Ordres** : tous les ordres passés (Exécuté, En attente, Annulé, Rejeté)
- **Transactions** : les échanges réellement exécutés (similaire aux ordres exécutés, mais vu du côté portefeuille)

On gère l'onglet actif avec un état local simple.

**9.2 — Liste des ordres**

Tableau affichant pour chaque ordre :
- Date/heure de création
- Symbole de la crypto avec icône
- Type (Achat/Vente), Mode (Marché/Limite)
- Quantité, Prix, Total
- Statut avec badge coloré
- Bouton "Annuler" uniquement si le statut est "En attente"

**9.3 — Annulation d'un ordre**

Quand l'utilisateur clique "Annuler", on appelle `cancelOrder(id)`. Si succès, on rafraîchit la liste localement (on met à jour le statut dans l'état sans recharger toute la liste).

**9.4 — Liste des transactions**

Similaire à la liste des ordres mais ne montre que les transactions exécutées. Pas d'action disponible.

---

## 10. Page Profil

### Objectif
Afficher les informations du compte de l'utilisateur connecté.

### Sous-objectifs

**10.1 — Composant Profil**

Affiche les informations récupérées depuis `GET /auth/me` :
- Username
- Email
- Rôle (User / Admin)
- Date d'inscription (formatée)
- Solde actuel

Les données viennent d'AuthContext (déjà chargées) + un appel `getMe()` pour avoir les données fraîches.

---

## 11. Composants utilitaires

### Objectif
Factoriser les patterns répétitifs (chargement, erreurs, formatage) pour ne pas les réécrire à chaque page.

### Sous-objectifs

**11.1 — Hook useFetch**

Un hook générique qui encapsule le pattern classique de chargement de données :
1. Prend une fonction async et une liste de dépendances
2. Gère les états `data`, `loading`, `error`
3. Annule la requête si le composant est démonté (via un flag `isMounted`)
4. Se réexécute si les dépendances changent

On l'utilise dans pratiquement toutes les pages pour éviter de dupliquer la gestion des états de chargement.

**11.2 — Composant DisplayMessage**

Un composant d'alerte réutilisable avec 4 niveaux : `info`, `success`, `warning`, `error`. Chacun a une couleur distincte. Il peut être fermé par l'utilisateur (bouton ×) ou disparaître automatiquement après un délai.

Utilisé pour : erreurs de formulaire, confirmations d'actions, messages serveur.

**11.3 — Composant Loader**

Un spinner simple avec un message optionnel. Affiché pendant les chargements de données. Style cohérent avec le thème rétro.

**11.4 — Utilitaires de formatage**

Dans `formatters.js` :
- `formatPrice(value)` — formate un nombre en USD avec le bon nombre de décimales (les petites cryptos peuvent avoir 6 décimales)
- `formatBalance(value)` — formate un solde en USD avec 2 décimales et le signe $
- `gainLossColor(value)` — retourne la variable CSS de couleur selon que la valeur est positive ou négative

**11.5 — Utilitaire coinIcons**

`getCoinIconURL(symbol)` retourne l'image correspondante à un symbole de crypto (BTC, ETH, SOL, ADA, DOGE). Si le symbole n'est pas reconnu, retourne une image générique. Les images sont dans `src/assets/images/coins/`.

---

## 12. Effet CRT

### Objectif
Ajouter un effet rétro optionnel qui simule un vieux moniteur CRT (scanlines, vignette, phosphor glow).

### Sous-objectifs

**12.1 — CRTContext**

Un context léger qui gère un booléen `enabled` :
- Persiste la préférence dans le localStorage
- Active/désactive les classes CSS sur `document.body` et `#app-root`
- Expose `toggle()` pour basculer l'effet

**12.2 — Composant CRTOverlay**

Un div absolu positionné par-dessus tout le contenu. Contient plusieurs couches superposées via CSS :
- `scanlines` — lignes horizontales semi-transparentes
- `phosphor` — légère lueur verte/bleue
- `glitch` — artefacts visuels occasionnels
- `sweep` — balayage périodique
- `flicker` — scintillement subtil

**12.3 — Animation d'allumage**

Quand l'effet est activé, une animation CSS imite l'allumage d'un tube cathodique : l'écran s'élargit verticalement depuis une ligne fine (`scaleY(0.01)`) jusqu'à la taille normale (`scaleY(1)`), avec une forte luminosité initiale qui se normalise.

**12.4 — Bouton de toggle dans la Sidebar**

Le bouton est placé en bas de la Sidebar avec une icône TV pixel art. Il change de couleur selon l'état actif/inactif. L'état actif est lisible immédiatement sans avoir besoin d'un tooltip.

---

## 13. Déploiement Docker

### Objectif
Conteneuriser le frontend pour qu'il puisse être déployé avec le reste de l'application via Docker Compose.

### Sous-objectifs

**13.1 — Dockerfile multi-stage**

On utilise un build en deux étapes :
1. **Build stage** : image Node.js, installe les dépendances, lance `vite build` → produit le dossier `dist/`
2. **Production stage** : image Nginx légère (Alpine), copie uniquement le `dist/` depuis l'étape précédente

L'image finale ne contient pas Node.js ni les `node_modules`, elle ne fait que servir des fichiers statiques. Résultat : une image bien plus légère.

**13.2 — Configuration Nginx**

Le fichier `nginx.conf` configure le serveur pour :
- Servir les fichiers statiques du dossier `dist/`
- Rediriger toutes les routes inconnues vers `index.html` — indispensable pour le routing côté client (SPA). Sans ça, un refresh sur `/market` retournerait une 404.
- Pas de cache agressif sur `index.html` (pour permettre les mises à jour sans vider le cache navigateur)

**13.3 — Variables d'environnement au build**

Vite injecte les variables d'environnement **au moment du build** (pas à l'exécution). Cela signifie que les URLs de l'API sont "baked in" dans le bundle JavaScript.

En pratique : les variables `VITE_API_BASE_URL`, `VITE_BASE_URL`, et `VITE_SIGNALR_HUB_URL` doivent être définies avant de lancer `vite build` (via un fichier `.env` ou des arguments `--build-arg` Docker).

---

## Récapitulatif des dépendances entre composants

```
main.jsx
└── BrowserRouter
    └── AuthProvider (AuthContext)
        └── App.jsx
            └── CRTProvider (CRTContext)
                ├── CRTOverlay          ← lit CRTContext
                └── Routes
                    ├── /login          → LoginPage
                    │     └── LoginForm
                    ├── /register       → RegisterPage
                    │     └── RegisterForm
                    └── ProtectedRoute  ← vérifie AuthContext
                          ├── Sidebar   ← lit CRTContext (toggle)
                          ├── Navbar    ← lit AuthContext (user, balance)
                          └── Outlet
                                ├── /market      → MarketPage
                                │     ├── useSignalR
                                │     └── MarketGrid → MarketItem[]
                                ├── /portfolio   → PortfolioPage
                                │     ├── PortfolioSummary
                                │     ├── PortfolioChart
                                │     ├── HoldingsList
                                │     └── PerformanceCard
                                ├── /trade/:symbol  → TradePage
                                │     ├── PriceChart (useSignalR)
                                │     └── TradeForm
                                ├── /portfolio/holdings/:symbol → HoldingDetails
                                ├── /history    → HistoryPage
                                └── /profil     → ProfilPage → Profil
```

---

## Points de vigilance récurrents

| Problème courant | Solution appliquée |
|---|---|
| Fuites mémoire sur les abonnements SignalR | Fermeture de la connexion dans le `return` du `useEffect` du hook `useSignalR` |
| Token expiré en milieu de session | Écoute de l'événement custom `'unauthorized'` dans AuthContext → déconnexion automatique |
| Balances/données obsolètes après un achat | Appel explicite à `refreshUser()` après chaque ordre réussi |
| Refresh navigateur sur une route `/market` → 404 | Fallback Nginx vers `index.html` dans `nginx.conf` |
| Variables Vite non définies en prod | `.env.example` fourni, variables injectées au build Docker via `ARG`/`ENV` |
| Collisions de classes CSS entre composants | CSS Modules systématiquement — chaque composant a son fichier `.module.css` |
