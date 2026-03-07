using AuthService.Dtos;
using AuthService.Services;
using CryptoSim.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthManagementService _authService;
    
    public AuthController(IAuthManagementService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(request);
        return CreatedAtAction(nameof(GetCurrentUser), new { id = user.Id }, new { message = "Utilisateur enregistré avec succès" });
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.GetUserId();
        var profil = await _authService.GetCurrentUserAsync(userId);
        
        return Ok(profil);
    }
    
    [HttpGet("balance")]
    [Authorize]
    public async Task<IActionResult> GetBalanceAsync()
    {
        var userId = User.GetUserId();
        var balance = await _authService.GetBalanceAsync(userId);
        return Ok(balance);
    }

    [HttpPut("balance")]
    [Authorize]
    public async Task<IActionResult> UpdateBalanceAsync([FromBody] UpdateBalanceRequest request)
    {
        var userId = User.GetUserId();
        await _authService.UpdateBalanceAsync(userId, request.Amount);
        return NoContent();
    }

}