using CryptoSim.Shared.Clients;
using CryptoSim.Shared.Constants;
using OrderService.Dtos.Clients;

namespace OrderService.Services.Clients;

public class AuthApiClient : BaseApiClient
{
    public AuthApiClient(HttpClient httpClient, ILogger logger, IConfiguration config) : base(httpClient, logger, config[EnvConstants.AuthServiceUrl]!) { }



    public async Task<decimal> GetUserBalance(string token)
    {
        var authResponseDto = await GetAsync<AuthApiResponseDto>($"api/auth/balance", token);

        return authResponseDto?.Balance ?? 0;

    }


    public async Task<bool> UpdateUserBalance(decimal amount, string token) 
    {

        var authResponse = await PutAsync<AuthApiUpdateBalanceRequestDto>($"api/auth/balance", new AuthApiUpdateBalanceRequestDto(Amount: amount), token);

        return authResponse;
    
    }

}
