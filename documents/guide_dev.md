## Développement :
1. AuthService (Indépendant, sécurité)
2. MarketService (Indépendant, temps réel)
3. PortfolioService (Dépendant de 1 et 2)
4. OrderService (Orchestrateur, dépendant de 1, 2 et 3)
5. API Gateway
6. React Frontend

---

### .env (ne pas commit)

```yml
# AUTHENTIFICATION
JWT_SECRET=JaiLaFlemmeDeChercherUneCleSecreteTresTresLongueAlorsJecristCa!!
JWT_ISSUER=CryptoSimAuth
JWT_AUDIENCE=CryptoSimServices
JWT_EXPIRATION_MINUTES=60

# URLs DES SERVICES
AUTH_SERVICE_URL=http://localhost:5001
MARKET_SERVICE_URL=http://localhost:5002
PORTFOLIO_SERVICE_URL=http://localhost:5003
ORDER_SERVICE_URL=http://localhost:5004

# BDD 
AUTH_DB_CONNECTION=Server=localhost;Database=cs_users_db;User=root;Password=formation;
MARKET_DB_CONNECTION=Server=localhost;Database=cs_market_db;User=root;Password=formation;
PORTFOLIO_DB_CONNECTION=Server=localhost;Database=cs_portfolio_db;User=root;Password=formation;
ORDER_DB_CONNECTION=Server=localhost;Database=cs_orders_db;User=root;Password=formation;

```
---

### Comment utiliser le .env dans le code

```C#
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Charger le fichier .env situé à la racine de la solution
Env.Load("..chemin_relatif/.env");

// Accéder aux variables comme si elles étaient dans appsettings.json
var jwtSecret = builder.Configuration["JWT_SECRET"];
var connectionString = builder.Configuration["AUTH_DB_CONNECTION"];
```


### Structure des projets

```
XyzService/
├── Controllers/       # Endpoints API (REST)
├── Data/              # DbContext (EF Core)
├── Dtos/              # Objets DTO (Request/Response)
├── Exceptions/        # Exceptions personnalisées
├── Extensions/        # Mappers (Model <-> Dto <-> Model)
├── Middlewares/       # Intercepteurs de requêtes (ex: ErrorHandlerMiddleware)
├── Models/            # Entités de BDD
├── Repositories/      # Accès aux données
├── Services/          # Logique métier
├── Program.cs
└── appsettings.json
```

