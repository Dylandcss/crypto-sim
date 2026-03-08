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

    [HttpPost("/api/auth/login")]
    public async Task<IActionResult> Login([FromBody] object body)
    {
        var client = CreateClient("AuthClient", addAuthTokenToHeader: false);
        var response = await client.PostAsJsonAsync("/api/auth/login", body);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpGet("/api/market/cryptos")]
    [Authorize]
    public async Task<IActionResult> GetCryptos()
    {
        var client = CreateClient("MarketClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/market/cryptos");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }

    [HttpGet("/api/portfolio")]
    [Authorize]
    public async Task<IActionResult> GetPortfolio()
    {
        var client = CreateClient("PortfolioClient", addAuthTokenToHeader: true);
        var response = await client.GetAsync("/api/portfolio");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }

    [HttpPost("/api/orders")]
    [Authorize]
    public async Task<IActionResult> CreateOrder([FromBody] object body)
    {
        var client = CreateClient("OrderClient", addAuthTokenToHeader: true);
        var response = await client.PostAsJsonAsync("/api/orders", body);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
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