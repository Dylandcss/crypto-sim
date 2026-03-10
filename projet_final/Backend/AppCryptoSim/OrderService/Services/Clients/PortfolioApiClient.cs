using CryptoSim.Shared.Clients;
using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Enums;
using OrderService.Dtos.Clients;

namespace OrderService.Services.Clients;

public class PortfolioApiClient : BaseApiClient
{

    public PortfolioApiClient(HttpClient httpClient, ILogger<PortfolioApiClient> logger, IConfiguration config) : base(httpClient, logger, config[EnvConstants.PortfolioServiceUrl]!) { }

    public async Task<decimal> GetHoldingQuantityAsync(string token, string symbol)
    {
        // On récupère l'objet Holding (qui contient la Quantity)
        var holding = await GetAsync<PortfolioApiHoldingResponseDto>($"api/portfolio/holdings/{symbol}", token);
        return holding?.Quantity ?? 0;
    }

    public async Task<bool> UpdateHoldingAsync(string token, string symbol, decimal quantity, decimal price, OrderType orderType)
    {
        var request = new UpdatePortfolioHoldingRequestDto(symbol, quantity, price, orderType);
        return await PostAsync<bool, UpdatePortfolioHoldingRequestDto>("api/portfolio/holdings", request, token);
    }

}
