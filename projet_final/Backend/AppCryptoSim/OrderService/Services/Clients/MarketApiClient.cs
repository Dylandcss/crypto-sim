using CryptoSim.Shared.Clients;
using CryptoSim.Shared.Constants;
using OrderService.Dtos.Clients;

namespace OrderService.Services.Clients;

public class MarketApiClient : BaseApiClient
{
    
    public MarketApiClient(HttpClient httpClient, ILogger<MarketApiClient> logger, IConfiguration config) : base(httpClient, logger, config[EnvConstants.MarketServiceUrl]!) { }


    public async Task<decimal> GetCryptoPriceAsync(string symbol, string token)
    {
        var cryptoResponseDto = await GetAsync<MarketApiResponseDto>($"api/market/cryptos/{symbol}", token);

        return cryptoResponseDto?.CurrentPrice ?? 0;
    }


}
