using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;


namespace CryptoSim.Shared.Clients;


public abstract class BaseApiClient
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;   

    protected BaseApiClient(HttpClient httpClient, ILogger logger, string baseAddress)
    {
        httpClient.BaseAddress = new Uri(baseAddress);
        _httpClient = httpClient;
        _logger = logger;
        
    }

    protected async Task<TResponse?> GetAsync<TResponse>(string endpoint, string? token = null)
    {
        return await SendRequestAsync<TResponse, object>(HttpMethod.Get, endpoint, null, token);
    }

    protected async Task<List<TResponse>> GetListAsync<TResponse>(string endpoint, string? token = null)
    {
        var result = await SendRequestAsync<List<TResponse>, object>(HttpMethod.Get, endpoint, null, token);
        return result ?? new List<TResponse>();
    }

    protected async Task<TResponse?> PostAsync<TResponse, TRequest>(string endpoint, TRequest bodyData, string? token = null)
    {
        return await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, endpoint, bodyData, token);
    }

    protected async Task<bool> PutAsync<TRequest>(string endpoint, TRequest bodyData, string? token = null)
    {
        var result = await SendRequestAsync<bool, TRequest>(HttpMethod.Put, endpoint, bodyData, token);
        return result;
    }

    protected async Task<TResponse?> GetInternalAsync<TResponse>(string endpoint, string internalKey)
        => await SendRequestAsync<TResponse, object>(HttpMethod.Get, endpoint, null, token: null, internalKey: internalKey);

    protected async Task<bool> PutInternalAsync<TRequest>(string endpoint, TRequest bodyData, string internalKey)
        => await SendRequestAsync<bool, TRequest>(HttpMethod.Put, endpoint, bodyData, token: null, internalKey: internalKey);

    protected async Task<TResponse?> PostInternalAsync<TResponse, TRequest>(string endpoint, TRequest bodyData, string internalKey)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, endpoint, bodyData, token: null, internalKey: internalKey);

    private async Task<TResponse?> SendRequestAsync<TResponse, TRequest>(HttpMethod method, string endpoint, TRequest? bodyData, string? token, string? internalKey = null)
    {
        try
        {
            var request = new HttpRequestMessage(method, endpoint);

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!string.IsNullOrEmpty(internalKey))
                request.Headers.Add("X-Internal-Key", internalKey);

            if (bodyData != null)
                request.Content = JsonContent.Create(bodyData);

            var response = await _httpClient.SendAsync(request);

            // Nettoyage !
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (response.IsSuccessStatusCode)
            {
                // Si on attend un booléen (pour PUT/DELETE), on retourne true
                if (typeof(TResponse) == typeof(bool)) return (TResponse)((object)true);

                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            _logger.LogWarning($"Appel {method} {endpoint} échoué. Code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            // Nettoyage impératif en cas d'erreur réseau
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _logger.LogError(ex, $"Erreur lors de l'appel {method} sur {endpoint}");
        }

        return default;
    }
}