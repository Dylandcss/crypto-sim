using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace CryptoSim.Shared.Extensions;

public static class ApplicationUrlExtensions
{

    /// <summary>
    /// Force le serveur à utiliser l'URL définie dans le fichier .env
    /// </summary>
    public static WebApplicationBuilder UseApplicationServiceUrlFromEnv(this WebApplicationBuilder builder, string envVarName)
    {
        // Recuperation de l'URL depuis la configuration (.env)
        var url = builder.Configuration[envVarName];
        
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentNullException($"La variable '{envVarName}' est introuvable. Vérifiez le fichier .env !");
        }

        builder.WebHost.UseUrls(url);

        return builder;
    }

}
