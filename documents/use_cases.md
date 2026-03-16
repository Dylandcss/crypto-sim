# Document des Use Cases - CryptoSim

## 1. Introduction

CryptoSim est une plateforme de **trading de cryptomonnaies simulé** basée sur une architecture **microservices**.
Elle permet aux utilisateurs de créer un compte, consulter les prix des cryptomonnaies, effectuer des ordres d’achat et de vente, et suivre la performance de leur portefeuille.

Les fonctionnalités sont réparties entre quatre microservices principaux :

* **AuthService** : gestion des comptes et authentification
* **MarketService** : gestion du marché et des prix
* **OrderService** : gestion des ordres d’achat et de vente
* **PortfolioService** : gestion du portefeuille et des transactions

L’acteur principal du système est :

**Utilisateur**

---

# 2. Liste globale des Use Cases

| ID   | Use Case                                | Acteur      | Service          |
| ---- | --------------------------------------- | ----------- | ---------------- |
| UC1  | Créer un compte                         | Utilisateur | AuthService      |
| UC2  | Se connecter                            | Utilisateur | AuthService      |
| UC3  | Consulter son profil                    | Utilisateur | AuthService      |
| UC4  | Consulter son solde                     | Utilisateur | AuthService      |
| UC5  | Consulter la liste des cryptomonnaies   | Utilisateur | MarketService    |
| UC6  | Voir les détails d’une cryptomonnaie    | Utilisateur | MarketService    |
| UC7  | Consulter l’historique des prix         | Utilisateur | MarketService    |
| UC8  | Voir les prix en temps réel             | Utilisateur | MarketService    |
| UC9  | Passer un ordre d’achat                 | Utilisateur | OrderService     |
| UC10 | Passer un ordre de vente                | Utilisateur | OrderService     |
| UC11 | Consulter l’historique des ordres       | Utilisateur | OrderService     |
| UC12 | Consulter les détails d’un ordre        | Utilisateur | OrderService     |
| UC13 | Annuler un ordre en attente             | Utilisateur | OrderService     |
| UC14 | Consulter son portefeuille              | Utilisateur | PortfolioService |
| UC15 | Voir les actifs détenus                 | Utilisateur | PortfolioService |
| UC16 | Consulter l’historique des transactions | Utilisateur | PortfolioService |
| UC17 | Consulter la performance globale        | Utilisateur | PortfolioService |

---

# 3. Use Cases détaillés

---

# UC1 - Créer un compte

### Acteur

Utilisateur

### Description

Permet à un nouvel utilisateur de créer un compte sur la plateforme afin d’accéder aux fonctionnalités de trading.

### Préconditions

* L’utilisateur ne possède pas encore de compte.

### Scénario principal

1. L’utilisateur accède à la page d’inscription.
2. Il saisit :

   * Username
   * Email
   * Mot de passe
3. Le système valide les informations.
4. Le mot de passe est chiffré.
5. Le compte est créé dans la base de données.
6. Un solde initial de **10 000 $ virtuels** est attribué.

### Postconditions

Le compte utilisateur est créé et prêt à être utilisé.

---

# UC2 - Se connecter

### Acteur

Utilisateur

### Description

Permet à un utilisateur existant de se connecter à la plateforme.

### Préconditions

* L’utilisateur possède un compte.

### Scénario principal

1. L’utilisateur saisit son **username** et son **mot de passe**.
2. Le système vérifie les identifiants.
3. Si les informations sont correctes :

   * un **token JWT** est généré.
4. Le token est envoyé au client.

### Postconditions

L’utilisateur est authentifié et peut accéder aux services sécurisés.

---

# UC3 - Consulter son profil

### Acteur

Utilisateur

### Description

Permet à l’utilisateur de consulter les informations de son compte.

### Informations affichées

* Username
* Email
* Rôle
* Date de création du compte

---

# UC4 - Consulter son solde

### Acteur

Utilisateur

### Description

Permet à l’utilisateur de consulter le montant de liquidités disponibles pour le trading.

### Informations affichées

* Solde actuel en dollars virtuels

---

# UC5 - Consulter la liste des cryptomonnaies

### Acteur

Utilisateur

### Description

Permet à l’utilisateur de voir toutes les cryptomonnaies disponibles sur la plateforme.

### Informations affichées

* Symbole
* Nom
* Prix actuel
* Dernière mise à jour

---

# UC6 - Voir les détails d’une cryptomonnaie

### Acteur

Utilisateur

### Description

Permet d’obtenir les informations détaillées d’une cryptomonnaie.

### Informations affichées

* Nom
* Symbole
* Prix actuel
* Dernière mise à jour

---

# UC7 - Consulter l’historique des prix

### Acteur

Utilisateur

### Description

Permet de consulter l’historique des variations de prix d’une cryptomonnaie.

### Informations affichées

* Liste des prix
* Date d’enregistrement
* évolution du prix

---

# UC8 - Voir les prix en temps réel

### Acteur

Utilisateur

### Description

Permet de recevoir les mises à jour de prix des cryptomonnaies en temps réel.

### Fonctionnement

* Les données sont diffusées via **SignalR**
* Les prix sont mis à jour toutes les **3 secondes**

---

# UC9 - Passer un ordre d’achat

### Acteur

Utilisateur

### Description

Permet à l’utilisateur d’acheter une cryptomonnaie.

### Préconditions

* L’utilisateur est connecté
* Le solde est suffisant

### Scénario principal

1. L’utilisateur choisit une cryptomonnaie.
2. Il saisit la quantité à acheter.
3. Le système récupère le prix actuel.
4. Le coût total est calculé.
5. Le solde est vérifié.
6. L’ordre est exécuté.

### Postconditions

* Le solde est débité.
* Le portefeuille est mis à jour.

---

# UC10 - Passer un ordre de vente

### Acteur

Utilisateur

### Description

Permet à un utilisateur de vendre une cryptomonnaie.

### Préconditions

* L’utilisateur possède la cryptomonnaie.

### Scénario principal

1. L’utilisateur sélectionne l’actif à vendre.
2. Il saisit la quantité.
3. Le système vérifie le portefeuille.
4. L’ordre est exécuté.

### Postconditions

* Le solde est crédité.
* Le portefeuille est mis à jour.

---

# UC11 - Consulter l’historique des ordres

### Acteur

Utilisateur

### Description

Permet de consulter tous les ordres passés.

### Informations affichées

* type d’ordre
* cryptomonnaie
* quantité
* prix
* statut
* date

---

# UC12 - Consulter les détails d’un ordre

### Acteur

Utilisateur

### Description

Permet de voir les informations complètes d’un ordre spécifique.

---

# UC13 - Annuler un ordre en attente

### Acteur

Utilisateur

### Description

Permet d’annuler un ordre qui n’a pas encore été exécuté.

### Préconditions

* L’ordre doit être **Pending**.

---

# UC14 - Consulter son portefeuille

### Acteur

Utilisateur

### Description

Affiche une vue globale du portefeuille.

### Informations affichées

* solde disponible
* valeur totale du portefeuille
* gain / perte total

---

# UC15 - Voir les actifs détenus

### Acteur

Utilisateur

### Description

Permet de voir toutes les cryptomonnaies détenues.

### Informations affichées

* symbole
* quantité
* prix moyen d’achat
* valeur actuelle
* gain/perte

---

# UC16 - Consulter l’historique des transactions

### Acteur

Utilisateur

### Description

Permet de consulter toutes les transactions effectuées.

### Informations affichées

* type (achat/vente)
* crypto
* quantité
* prix
* date

---

# UC17 - Consulter la performance globale

### Acteur

Utilisateur

### Description

Permet de voir la performance globale du portefeuille.

### Calculs affichés

* investissement total
* valeur actuelle
* gain / perte
* pourcentage de gain / perte


