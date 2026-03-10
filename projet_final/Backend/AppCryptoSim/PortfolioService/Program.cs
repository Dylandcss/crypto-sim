using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Extensions;
using PortfolioService.Services.Clients;
using PortfolioService.Data;
using PortfolioService.Repositories;
using PortfolioService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.AddMicroserviceCore(EnvConstants.PortfolioServiceUrl)
    .AddMysqlDatabase<PortfolioDbContext>(EnvConstants.PortfolioDb);

builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IPortfolioManagementService, PortfolioManagementService>();

builder.Services.AddHttpClient<MarketApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
