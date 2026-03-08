using Microsoft.AspNetCore.Http;


namespace CryptoSim.Shared.Extensions;


public static class HttpContextExtensions
{
    /// <summary>
    /// Extrait proprement le token JWT du header Authorization
    /// </summary>
    public static string GetBearerToken(this HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return authHeader["Bearer ".Length..].Trim();
    }
}
