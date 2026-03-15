namespace CryptoSim.Shared.Constants;

public static class EnvConstants
{
    // URLs des Services
    public const string AuthServiceUrl = "AUTH_SERVICE_URL";
    public const string MarketServiceUrl = "MARKET_SERVICE_URL";
    public const string PortfolioServiceUrl = "PORTFOLIO_SERVICE_URL";
    public const string OrderServiceUrl = "ORDER_SERVICE_URL";
    public const string GatewayUrl = "GATEWAY_URL";
    
    // JWT
    public const string JwtSecret = "JWT_SECRET";
    public const string JwtIssuer = "JWT_ISSUER";
    public const string JwtAudience = "JWT_AUDIENCE";
    public const string JwtExpiration = "JWT_EXPIRATION_MINUTES";

    // BDD
    public const string AuthDb = "AUTH_DB_CONNECTION";
    public const string MarketDb = "MARKET_DB_CONNECTION";
    public const string PortfolioDb = "PORTFOLIO_DB_CONNECTION";
    public const string OrderDb = "ORDER_DB_CONNECTION";

    // FRONT
    public const string FrontendUrl = "FRONTEND_URL";
    public const string CorsPolicyName = "AllowReactApp";

    // HUB
    public const string MarketHubPath = "MARKET_HUB_PATH";

    // Communication inter-services
    public const string InternalApiKey = "INTERNAL_API_KEY";
}
