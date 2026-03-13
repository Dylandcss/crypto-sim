using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Extensions;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.AddMicroserviceCore(EnvConstants.GatewayUrl)
       .AddFrontendCors();


var authServiceURI = new Uri(builder.Configuration[EnvConstants.AuthServiceUrl]!);
var marketServiceURI = new Uri(builder.Configuration[EnvConstants.MarketServiceUrl]!);
var portfolioServiceURI = new Uri(builder.Configuration[EnvConstants.PortfolioServiceUrl]!);
var orderServiceURI = new Uri(builder.Configuration[EnvConstants.OrderServiceUrl]!);

var marketHubPath = builder.Configuration[EnvConstants.MarketHubPath] ?? "/marketHub";
if (!marketHubPath.StartsWith('/'))
{
    marketHubPath = $"/{marketHubPath}";
}

var marketHubBasePath = marketHubPath.TrimEnd('/');
if (string.IsNullOrWhiteSpace(marketHubBasePath))
{
    marketHubBasePath = "/marketHub";
}

builder.Services.AddHttpClient("AuthClient", client => client.BaseAddress = authServiceURI);
builder.Services.AddHttpClient("MarketClient", client => client.BaseAddress = marketServiceURI);
builder.Services.AddHttpClient("PortfolioClient", client => client.BaseAddress = portfolioServiceURI);
builder.Services.AddHttpClient("OrderClient", client => client.BaseAddress = orderServiceURI);

// Reverse proxy pour le MarketHub (SignalR) pour forcer les connexions par la Gateway
builder.Services.AddReverseProxy().LoadFromMemory(
[
    new RouteConfig
    {
        RouteId = "market-hub-root",
        ClusterId = "market-hub-cluster",
        Match = new RouteMatch { Path = marketHubBasePath }
    },
    new RouteConfig
    {
        RouteId = "market-hub-catch-all",
        ClusterId = "market-hub-cluster",
        Match = new RouteMatch { Path = $"{marketHubBasePath}/{{**catch-all}}" }
    }
],
[
    new ClusterConfig
    {
        ClusterId = "market-hub-cluster",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            ["market-service"] = new() { Address = $"{marketServiceURI.Scheme}://{marketServiceURI.Authority}/" }
        }
    }
]);


var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseFrontendCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();

app.Run();