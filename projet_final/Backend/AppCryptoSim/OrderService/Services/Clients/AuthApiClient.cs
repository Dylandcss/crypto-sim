using CryptoSim.Shared.Clients;
using CryptoSim.Shared.Constants;
using OrderService.Dtos.Clients;

namespace OrderService.Services.Clients;

public class AuthApiClient : BaseApiClient
{
    private readonly string _internalKey;

    public AuthApiClient(HttpClient httpClient, ILogger<AuthApiClient> logger, IConfiguration config)
        : base(httpClient, logger, config[EnvConstants.AuthServiceUrl]!)
    {
        _internalKey = config[EnvConstants.InternalApiKey] ?? string.Empty;
    }

    public async Task<decimal> GetUserBalance(string token)
    {
        var authResponseDto = await GetAsync<AuthApiResponseDto>("api/auth/balance", token);
        return authResponseDto?.Balance ?? 0;
    }

    public async Task<bool> UpdateUserBalance(decimal amount, string token)
    {
        return await PutAsync<AuthApiUpdateBalanceRequestDto>("api/auth/balance", new AuthApiUpdateBalanceRequestDto(Amount: amount), token);
    }

    // --- Appels internes (background service, pas de JWT utilisateur) ---

    public async Task<decimal> GetUserBalanceInternal(int userId)
    {
        var result = await GetInternalAsync<AuthApiResponseDto>($"api/auth/internal/balance/{userId}", _internalKey);
        return result?.Balance ?? 0;
    }

    public async Task<bool> UpdateUserBalanceInternal(int userId, decimal amount)
    {
        return await PutInternalAsync($"api/auth/internal/balance/{userId}", new AuthApiUpdateBalanceRequestDto(Amount: amount), _internalKey);
    }
}
