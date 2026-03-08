using CryptoSim.Shared.Clients;
using OrderService.Dtos;

namespace OrderService.Services.Clients;

public class MarketApiClient : BaseApiClient
{
    
    public MarketApiClient(HttpClient httpClient, ILogger logger) : base(httpClient, logger) { }


    public async Task<decimal> GetCryptoPriceAsync(string symbol, string token)
    {
        var cryptoResponseDto = await GetAsync<MarketApiResponseDto>($"api/market/cryptos/{symbol}", token);

        return cryptoResponseDto?.CurrentPrice ?? 0;
    }


}
