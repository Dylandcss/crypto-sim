using CryptoSim.Shared.Exceptions;
using System.Security.Claims;

namespace CryptoSim.Shared.Extensions;

public static class ClaimsExtensions
{

    /// <summary>
    /// Permet de récuperer l'id de l'utilisateur connecté depuis le Token JWT.
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal claim)
    {
        var idClaim = claim.FindFirstValue(ClaimTypes.NameIdentifier);

        if(idClaim is  null || !int.TryParse(idClaim, out int userId))
        {
            throw new UnauthorizedException("Identifiant utilisateur introuvable dans le token.");
        }

        return userId;
    }



}
