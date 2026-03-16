using CryptoSim.Shared.Extensions; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
using OrderService.Services.Interfaces;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {        
        var token = HttpContext.GetBearerToken();
        var userId = User.GetUserId();

        var response = await _orderService.AddOrderAsync(userId, request, token);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserOrders()
    {
        var userId = User.GetUserId();
        return Ok(await _orderService.GetUserOrdersAsync(userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _orderService.GetOrderByIdAsync(id, userId));
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderById(int id)
    {
        var userId = User.GetUserId();
        await _orderService.DeleteOrderAsync(id, userId);
        return NoContent();
    }
}