using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Models;
using CryptoSim.Shared.Constants;
using Microsoft.IdentityModel.Tokens;


namespace AuthService.Services;


public class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration configuration)
    {
        _config = configuration;
    }

    public (string Token, int ExpirationMinutes) GenerateToken(User user)
    {

        var secretKey = _config[EnvConstants.JwtSecret] ?? throw new InvalidOperationException($"La clé {EnvConstants.JwtSecret} est manquante.");

        var expirationMinutes = int.Parse(_config[EnvConstants.JwtExpiration] ?? "1440");
        var issuer = _config[EnvConstants.JwtIssuer];
        var audience = _config[EnvConstants.JwtAudience];

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),

            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),

            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

            Issuer = issuer,
            Audience = audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expirationMinutes);
    }
}