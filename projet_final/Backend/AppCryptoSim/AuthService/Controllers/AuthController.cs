using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Service d'authentification injecté via le constructeur
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
        return Ok(user);
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null)
        {
            return Unauthorized("User ID not found in token.");
        }
        
        var userId = int.Parse(userIdClaim.Value);
        var profil = await _authService.GetCurrentUserAsync(userId);
        
        return Ok(profil);
    }
    
    [HttpGet("balance")]
    [Authorize]
    public async Task<IActionResult> GetBalanceAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID not found in token.");
        }
        
        var userId = int.Parse(userIdClaim.Value);
        var balance = await _authService.GetBalanceAsync(userId);
        return Ok(balance);
    }

    [HttpPut("balance")]
    [Authorize]
    public async Task<IActionResult> UpdateBalanceAsync([FromBody] double balance)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User ID not found in token.");
        
        var userId = int.Parse(userIdClaim.Value);
        var amount = Convert.ToDecimal(balance);

        await _authService.UpdateBalanceAsync(userId, amount);
        return NoContent();
    }
}