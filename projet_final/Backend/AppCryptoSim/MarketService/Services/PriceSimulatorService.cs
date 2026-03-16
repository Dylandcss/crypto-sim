using MarketService.Dtos;
using MarketService.Extensions;
using MarketService.Services.Hubs;
using MarketService.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;


namespace MarketService.Services;

public class PriceSimulatorService : BackgroundService
{

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PriceSimulatorService> _logger;
    private readonly int _updateInterval;
    private readonly IHubContext<MarketHub> _hubContext;

    public PriceSimulatorService(
        IServiceScopeFactory scopeFactory,
        ILogger<PriceSimulatorService> logger,
        IConfiguration configuration,
        IHubContext<MarketHub> hubContext
    ) {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _updateInterval = configuration.GetValue<int>("MARKET_UPDATE_INTERVAL", 3000);
        _hubContext = hubContext;
    }


    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Simulateur Mrket lancé...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try { 
                using (var scope = _scopeFactory.CreateScope())
                { 
                    var cryptoService = scope.ServiceProvider.GetRequiredService<ICryptosService>();
                    var priceHistoryService = scope.ServiceProvider.GetRequiredService<IPriceHistoryService>();

                    var cryptos = await cryptoService.GetAllCryptosAsync();
                    var updates = new List<PriceUpdate>();

                    foreach (var crypto in cryptos) 
                    { 
                        var randomValue = (decimal) ((Random.Shared.NextDouble() * 4) - 2) / 100.0m;
                        var newPrice = Math.Max(0.0001m, crypto.CurrentPrice * (1 +  randomValue));

                        await cryptoService.UpdatePriceAsync(crypto.Id, newPrice);
                        await priceHistoryService.AddNewPriceHistoryAsync(crypto.Symbol, newPrice);

                        updates.Add(crypto.ToDto(newPrice));
                    }

                    await _hubContext.Clients.All.SendAsync("ReceivePrices", updates, stoppingToken);
                    _logger.LogInformation($"Prix mis à jour pour {cryptos.Count} cryptos.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans le simulateur de market. Tentative de reprise dans 3s...");
            }

            await Task.Delay(_updateInterval, stoppingToken);
        }
    }
}
