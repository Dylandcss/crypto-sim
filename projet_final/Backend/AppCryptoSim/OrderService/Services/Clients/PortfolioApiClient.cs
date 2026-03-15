using CryptoSim.Shared.Clients;
using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Enums;
using OrderService.Dtos.Clients;

namespace OrderService.Services.Clients;

public class PortfolioApiClient : BaseApiClient
{
    private readonly string _internalKey;

    public PortfolioApiClient(HttpClient httpClient, ILogger<PortfolioApiClient> logger, IConfiguration config)
        : base(httpClient, logger, config[EnvConstants.PortfolioServiceUrl]!)
    {
        _internalKey = config[EnvConstants.InternalApiKey] ?? string.Empty;
    }

    public async Task<decimal> GetHoldingQuantityAsync(string token, string symbol)
    {
        var holding = await GetAsync<PortfolioApiHoldingResponseDto>($"api/portfolio/holdings/{symbol}", token);
        return holding?.Quantity ?? 0;
    }

    public async Task<bool> UpdateHoldingAsync(string token, string symbol, decimal quantity, decimal price, OrderType orderType)
    {
        var request = new UpdatePortfolioHoldingRequestDto(symbol, quantity, price, orderType);
        return await PostAsync<bool, UpdatePortfolioHoldingRequestDto>("api/portfolio/holdings", request, token);
    }

    // --- Appels internes (background service, pas de JWT utilisateur) ---

    public async Task<decimal> GetHoldingQuantityInternalAsync(int userId, string symbol)
    {
        var holding = await GetInternalAsync<PortfolioApiHoldingResponseDto>($"api/portfolio/internal/holdings/{symbol}/quantity/{userId}", _internalKey);
        return holding?.Quantity ?? 0;
    }

    public async Task<bool> UpdateHoldingInternalAsync(int userId, string symbol, decimal quantity, decimal price, OrderType orderType)
    {
        var request = new InternalUpdatePortfolioHoldingRequestDto(userId, symbol, quantity, price, orderType);
        return await PostInternalAsync<bool, InternalUpdatePortfolioHoldingRequestDto>("api/portfolio/internal/holdings", request, _internalKey);
    }
}
