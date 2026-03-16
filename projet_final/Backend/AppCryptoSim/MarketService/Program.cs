using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Extensions;
using MarketService.Data;
using MarketService.Repositories;
using MarketService.Repositories.Interfaces;
using MarketService.Services;
using MarketService.Services.Hubs;
using MarketService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.AddMicroserviceCore(EnvConstants.MarketServiceUrl)
       .AddFrontendCors()
       .AddMysqlDatabase<MarketDbContext>(EnvConstants.MarketDb);

builder.Services.AddSignalR();

builder.Services
    .AddScoped<ICryptosRepository, CryptosRepository>()
    .AddScoped<IPriceHistoryRepository, PriceHistoryRepository>()
    .AddScoped<ICryptosService, CryptosService>()
    .AddScoped<IPriceHistoryService, PriceHistoryService>()
    .AddHostedService<PriceSimulatorService>();

var app = builder.Build();

app.UseGlobalExceptionHandler()
   .UseFrontendCors()
   .UseAuthentication()
   .UseAuthorization();

app.MapControllers();


var marketHubPath = builder.Configuration[EnvConstants.MarketHubPath] ?? "/marketHub";
app.MapHub<MarketHub>(marketHubPath);

app.Run();