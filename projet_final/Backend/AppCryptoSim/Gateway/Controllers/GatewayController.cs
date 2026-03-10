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
    public async Task<IActionResult> GetCurrentUser()
    {
        var client = CreateClient("AuthClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/auth/me");
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }
    
    [HttpGet("/api/auth/balance")]
    [Authorize]
    public async Task<IActionResult> GetBalanceAsync()
    {
        var client = CreateClient("AuthClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/auth/balance");
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }
    
    [HttpPut("/api/auth/balance")]
    [Authorize]
    public async Task<IActionResult> UpdateBalanceAsync([FromBody] object request)
    {
        var client = CreateClient("AuthClient", addAuthTokenToHeader: true);
        var response = await client.PutAsJsonAsync("/api/auth/balance", request);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }
    
    [HttpPost("/api/auth/register")]
    public async Task<IActionResult> Register([FromBody] object body)
    {
        var client = CreateClient("AuthClient", addAuthTokenToHeader: true);
        var response = await client.PostAsJsonAsync("/api/auth/register", body);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost("/api/auth/login")]
    public async Task<IActionResult> Login([FromBody] object body)
    {
        var client = CreateClient("AuthClient", addAuthTokenToHeader: false);
        var response = await client.PostAsJsonAsync("/api/auth/login", body);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpGet("/api/market/cryptos")]
    public async Task<IActionResult> GetCryptos()
    {
        var client = CreateClient("MarketClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/market/cryptos");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpGet("/api/market/cryptos/{symbol}")]
    public async Task<IActionResult> GetCryptoBySymbolAsync(string symbol) 
    {
        var client = CreateClient("MarketClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync($"/api/market/cryptos/{symbol}");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpGet("/api/market/history/{symbol}")]
    public async Task<IActionResult> GetCryptoPriceHistoryAsync(string symbol, [FromQuery] int limit = 50)
    {
        var client = CreateClient("MarketClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync($"/api/market/history/{symbol}?limit={limit}");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpGet("/api/market/snapshot")]
    public async Task<IActionResult> GetCryptoSnapshot(DateTime date)
    {
        var client = CreateClient("MarketClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync($"/api/market/snapshot?date={date:O}");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpPost("/api/orders")]
    [Authorize]
    public async Task<IActionResult> CreateOrder([FromBody] object request)
    {        
        var client = CreateClient("OrderClient", addAuthTokenToHeader: true);
        var response = await client.PostAsJsonAsync("/api/orders", request);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }
    
    [HttpGet("/api/orders")]
    [Authorize]
    public async Task<IActionResult> GetUserOrders()
    {
        var client = CreateClient("OrderClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/orders");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpGet("/api/orders/{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var client = CreateClient("OrderClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync($"/api/orders/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpDelete("/api/orders/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteOrderById(int id)
    {
        var client = CreateClient("OrderClient", addAuthTokenToHeader: true);
        var response = await client.DeleteAsync($"/api/orders/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }
    
    [HttpGet("/api/portfolio")]
    [Authorize]
    public async Task<IActionResult> GetPortfolioSummary()
    {
        var client = CreateClient("PortfolioClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/portfolio");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpGet("/api/portfolio/holdings")]
    [Authorize]
    public async Task<ActionResult<List<object>>> GetHoldings()
    {
        var client = CreateClient("PortfolioClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/portfolio/holdings");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpGet("/api/portfolio/holdings/{symbol}")]
    [Authorize]
    public async Task<ActionResult<List<object>>> GetHolding(string symbol)
    {
        var client = CreateClient("PortfolioClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync($"/api/portfolio/holdings/{symbol}");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpGet("/api/portfolio/transactions")]
    [Authorize]
    public async Task<ActionResult<List<object>>> GetTransactions()
    {
        var client = CreateClient("PortfolioClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/portfolio/transactions");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    [HttpGet("/api/portfolio/performance")]
    [Authorize]
    public async Task<ActionResult<object>> GetPerformance()
    {
        var client = CreateClient("PortfolioClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/portfolio/performance");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }
    
    private HttpClient CreateClient(string clientName, bool addAuthTokenToHeader)
    {
        var client = _httpClientFactory.CreateClient(clientName);

        if (addAuthTokenToHeader)
        {
            var authHeader = HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader["Bearer ".Length..].Trim();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return client;
    }
}