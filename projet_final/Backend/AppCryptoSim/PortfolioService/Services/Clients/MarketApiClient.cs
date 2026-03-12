using CryptoSim.Shared.Clients;
using CryptoSim.Shared.Constants;
using OrderService.Dtos.Clients;

namespace PortfolioService.Services.Clients;

public class MarketApiClient : BaseApiClient
{
    public MarketApiClient(HttpClient httpClient, ILogger<MarketApiClient> logger, IConfiguration config) 
        : base(httpClient, logger, config[EnvConstants.MarketServiceUrl]!) 
    { }

    public async Task<MarketApiResponseDto> GetCryptoAsync(string symbol, string token)
    {
        var cryptoResponseDto = await GetAsync<MarketApiResponseDto>($"api/market/cryptos/{symbol}", token);
        if (cryptoResponseDto != null)
            return new MarketApiResponseDto(cryptoResponseDto.Symbol, cryptoResponseDto.Name,
                cryptoResponseDto.CurrentPrice);
        return new MarketApiResponseDto(symbol, "Unknown", 0m);
    }
    
    public async Task<List<MarketApiResponseDto>> GetCryptosAsync(string token)
    {
        var cryptosResponse = await GetAsync<List<MarketApiResponseDto>>("api/market/cryptos", token);
        return cryptosResponse ?? new List<MarketApiResponseDto>();
    }
}