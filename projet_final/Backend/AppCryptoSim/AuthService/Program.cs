using System.Text;
using AuthService.Data;
using AuthService.Middlewares;
using AuthService.Repositories;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

Env.Load("../.env");
var builder = WebApplication.CreateBuilder(args);

// Charger le fichier .env situé à la racine de la solution
builder.Configuration.AddEnvironmentVariables();

// Accéder aux variables d'environnement
var connectionString = builder.Configuration["AUTH_DB_CONNECTION"];

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });

// Injection des dépendances
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthManagementService, AuthManagementService>();
builder.Services.AddSingleton(new JwtService(builder.Configuration));


// Configuration de l'authentification JWT
var secretKey = builder.Configuration["JWT_SECRET"]
                ?? throw new InvalidOperationException("JWT_SECRET manquant.");

var key = Encoding.ASCII.GetBytes(secretKey);

var issuer = builder.Configuration["JWT_ISSUER"];
var audience = builder.Configuration["JWT_AUDIENCE"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

//Ajout du DbContext avec Mysql
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

// Vérifier le header Authorization
app.UseAuthentication(); 

// Vérifier les attributs [Authorize] et les rôles
app.UseAuthorization();

app.MapControllers();

app.Run();
