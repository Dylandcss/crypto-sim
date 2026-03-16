using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Gateway.Controllers;

[ApiController]
public class GatewayController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GatewayController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpGet("/api/auth/me")]
    [Authorize]
    public Task<IActionResult> GetCurrentUser() 
        => ForwardRequestAsync("AuthClient", "/api/auth/me", HttpMethod.Get);
    
    [HttpGet("/api/auth/balance")]
    [Authorize]
    public Task<IActionResult> GetBalanceAsync() 
        => ForwardRequestAsync("AuthClient", "/api/auth/balance", HttpMethod.Get);
    
    [HttpPut("/api/auth/balance")]
    [Authorize]
    public Task<IActionResult> UpdateBalanceAsync([FromBody] object request) 
        => ForwardRequestAsync("AuthClient", "/api/auth/balance", HttpMethod.Put, request);
    
    [HttpPost("/api/auth/register")]
    public Task<IActionResult> Register([FromBody] object body) 
        => ForwardRequestAsync("AuthClient", "/api/auth/register", HttpMethod.Post, body);

    [HttpPost("/api/auth/login")]
    public Task<IActionResult> Login([FromBody] object body) 
        => ForwardRequestAsync("AuthClient", "/api/auth/login", HttpMethod.Post, body, addAuthToken: false);

    [HttpGet("/api/market/cryptos")]
    public Task<IActionResult> GetCryptos() 
        => ForwardRequestAsync("MarketClient", "/api/market/cryptos", HttpMethod.Get);
    
    [HttpGet("/api/market/cryptos/{symbol}")]
    public Task<IActionResult> GetCryptoBySymbolAsync(string symbol) 
        => ForwardRequestAsync("MarketClient", $"/api/market/cryptos/{symbol}", HttpMethod.Get);
    
    [HttpGet("/api/market/history/{symbol}")]
    public Task<IActionResult> GetCryptoPriceHistoryAsync(string symbol, [FromQuery] int limit = 50) 
        => ForwardRequestAsync("MarketClient", $"/api/market/history/{symbol}?limit={limit}", HttpMethod.Get);
    
    [HttpGet("/api/market/snapshot")]
    public Task<IActionResult> GetCryptoSnapshot(DateTime date) 
        => ForwardRequestAsync("MarketClient", $"/api/market/snapshot?date={date:O}", HttpMethod.Get);
    
    [HttpPost("/api/orders")]
    [Authorize]
    public Task<IActionResult> CreateOrder([FromBody] object request) 
        => ForwardRequestAsync("OrderClient", "/api/orders", HttpMethod.Post, request);
    
    [HttpGet("/api/orders")]
    [Authorize]
    public Task<IActionResult> GetUserOrders() 
        => ForwardRequestAsync("OrderClient", "/api/orders", HttpMethod.Get);
    
    [HttpGet("/api/orders/{id:int}")]
    [Authorize]
    public Task<IActionResult> GetOrderById(int id) 
        => ForwardRequestAsync("OrderClient", $"/api/orders/{id}", HttpMethod.Get);
    
    [HttpDelete("/api/orders/{id:int}")]
    [Authorize]
    public Task<IActionResult> DeleteOrderById(int id) 
        => ForwardRequestAsync("OrderClient", $"/api/orders/{id}", HttpMethod.Delete);
    
    [HttpGet("/api/portfolio")]
    [Authorize]
    public Task<IActionResult> GetPortfolioSummary() 
        => ForwardRequestAsync("PortfolioClient", "/api/portfolio", HttpMethod.Get);
    
    [HttpGet("/api/portfolio/holdings")]
    [Authorize]
    public Task<IActionResult> GetHoldings() 
        => ForwardRequestAsync("PortfolioClient", "/api/portfolio/holdings", HttpMethod.Get);
    
    [HttpGet("/api/portfolio/holdings/{symbol}")]
    [Authorize]
    public Task<IActionResult> GetHolding(string symbol) 
        => ForwardRequestAsync("PortfolioClient", $"/api/portfolio/holdings/{symbol}", HttpMethod.Get);
    
    [HttpGet("/api/portfolio/transactions")]
    [Authorize]
    public Task<IActionResult> GetTransactions() 
        => ForwardRequestAsync("PortfolioClient", "/api/portfolio/transactions", HttpMethod.Get);
    
    [HttpGet("/api/portfolio/performance")]
    [Authorize]
    public Task<IActionResult> GetPerformance() 
        => ForwardRequestAsync("PortfolioClient", "/api/portfolio/performance", HttpMethod.Get);


    private async Task<IActionResult> ForwardRequestAsync(string clientName, string path, HttpMethod method, object? bodyData = null, bool addAuthToken = true)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        using var request = new HttpRequestMessage(method, path);

        if (bodyData != null)
        {
            request.Content = JsonContent.Create(bodyData);
        }

        if (addAuthToken)
        {
            var authHeader = HttpContext.Request.Headers.Authorization.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader["Bearer ".Length..].Trim();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int) response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }
}