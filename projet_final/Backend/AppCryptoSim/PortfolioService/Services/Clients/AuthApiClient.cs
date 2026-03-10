using CryptoSim.Shared.Clients;
using CryptoSim.Shared.Constants;
using PortfolioService.Dtos.Clients;

namespace PortfolioService.Services.Clients;

public class AuthApiClient : BaseApiClient
{
    public AuthApiClient(HttpClient httpClient, ILogger<AuthApiClient> logger, IConfiguration config) 
        : base(httpClient, logger, config[EnvConstants.MarketServiceUrl]!) 
    { }

    public async Task<decimal> GetUserBalance(string token)
    {
        var result = await GetAsync<AuthApiResponseDto>($"api/auth/balance", token);
        return result?.balance ?? 0;
    }
}