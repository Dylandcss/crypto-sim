using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace CryptoSim.Shared.Extensions;

public static class MicroserviceExtensions
{
    /// <summary>
    /// Configure le socle technique pour tout microservice
    /// </summary>
    public static WebApplicationBuilder AddMicroserviceCore(this WebApplicationBuilder builder, string envServiceUrlKey)
    {
        // 1. Chargement du .env
        Env.Load("../.env");
        builder.Configuration.AddEnvironmentVariables();

        // Configuration de l'URL:Port
        builder.UseApplicationServiceUrlFromEnv(envServiceUrlKey);

        // Configuration des services JWT + MVC
        builder.Services
            .AddSharedJwtAuthentication(builder.Configuration)
            .AddControllers()
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return builder;
    }

    /// <summary>
    /// Configure CORS pour autoriser le Frontend (React).
    /// </summary>
    public static WebApplicationBuilder AddFrontendCors(this WebApplicationBuilder builder, string frontendUrl)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                policy.WithOrigins(frontendUrl)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
        return builder;
    }

    /// <summary>
    /// Configure la connexion à la base de données MySQL.
    /// </summary>
    public static WebApplicationBuilder AddMysqlDatabase<T>(this WebApplicationBuilder builder, string envConnectionStringKey) where T : DbContext
    {
        var connectionString = builder.Configuration[envConnectionStringKey];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException($"La chaîne de connexion '{envConnectionStringKey}' est introuvable dans le .env.");

        builder.Services.AddDbContext<T>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        return builder;
    }
}