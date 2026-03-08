using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.AddMicroserviceCore(EnvConstants.GatewayUrl)
       .AddFrontendCors();


var authServiceURI = new Uri(builder.Configuration[EnvConstants.AuthServiceUrl]!);
var marketServiceURI = new Uri(builder.Configuration[EnvConstants.MarketServiceUrl]!);
var portfolioServiceURI = new Uri(builder.Configuration[EnvConstants.PortfolioServiceUrl]!);
var orderServiceURI = new Uri(builder.Configuration[EnvConstants.OrderServiceUrl]!);


builder.Services.AddHttpClient("AuthClient", client => client.BaseAddress = authServiceURI);
builder.Services.AddHttpClient("MarketClient", client => client.BaseAddress = marketServiceURI);
builder.Services.AddHttpClient("PortfolioClient", client => client.BaseAddress = portfolioServiceURI);
builder.Services.AddHttpClient("OrderClient", client => client.BaseAddress = orderServiceURI);


var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseFrontendCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();