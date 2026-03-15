using OrderService.Services.Interfaces;

namespace OrderService.Services;

public class LimitOrderExecutorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LimitOrderExecutorService> _logger;

    public LimitOrderExecutorService(IServiceScopeFactory scopeFactory, ILogger<LimitOrderExecutorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                await orderService.ExecuteLimitOrdersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification des ordres limites.");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}
