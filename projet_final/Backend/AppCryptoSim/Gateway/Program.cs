using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

Env.Load("../.env");
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();


// Charger le fichier .env situé à la racine de la solution


// Accéder aux variables comme si elles étaient dans appsettings.json

//var secretKey = builder.Configuration["JWT_SECRET"]
                //?? throw new InvalidOperationException("JWT_SECRET manquant.");

string issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
string audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
string secret = Environment.GetEnvironmentVariable("JWT_SECRET");
var secretKey = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateLifetime = true
        };
    });

// Config des HttpClient
builder.Services.AddHttpClient("AuthClient", client => client.BaseAddress = new Uri("http://localhost:5001"));

builder.Services.AddHttpClient("MarketClient", client => client.BaseAddress = new Uri("http://localhost:5002"));

builder.Services.AddHttpClient("OrderClient", client => client.BaseAddress = new Uri("http://localhost:5003"));

builder.Services.AddHttpClient("PortfolioClient", client => client.BaseAddress = new Uri("http://localhost:5003"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
