using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Gateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GatewayController : ControllerBase
{

    private readonly IHttpClientFactory _httpClientFactory;

    public GatewayController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private void _AddAuthTokenToHeader(HttpClient client)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    [HttpPost("/api/auth/login")]
    public async Task<IActionResult> Login([FromBody] object body)
    {
        var client = _httpClientFactory.CreateClient("AuthClient");

        var jsonContent = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(body),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("/api/auth/login", jsonContent);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }


    [HttpGet("/api/market")]
    public async Task<IActionResult> GetMarket()
    {
        var client = _httpClientFactory.CreateClient("MarketClient");

        var response = await client.GetAsync("/api/market");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }

    [HttpGet("/api/order")]
    [Authorize]
    public async Task<IActionResult> GetOrder()
    {
        var client = _httpClientFactory.CreateClient("OrderClient");

        _AddAuthTokenToHeader(client);

        var response = await client.GetAsync("/api/order");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json", System.Text.Encoding.UTF8);
    }

    [HttpPost("/api/portfolio")]
    [Authorize]
    public async Task<IActionResult> CreatePortfolio([FromBody] object body)
    {
        var client = _httpClientFactory.CreateClient("PortfolioClient");
        _AddAuthTokenToHeader(client);

        var jsonContent = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(body),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("/api/portfolio", jsonContent);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

}