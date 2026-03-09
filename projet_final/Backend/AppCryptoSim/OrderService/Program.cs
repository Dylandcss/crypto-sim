using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Extensions;
using OrderService.Data;
using OrderService.Repositories;
using OrderService.Repositories.Interfaces;
using OrderService.Services;
using OrderService.Services.Clients;
using OrderService.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder
    .AddMicroserviceCore(EnvConstants.OrderServiceUrl)
    .AddMysqlDatabase<OrderDbContext>(EnvConstants.OrderDb);


builder.Services
    .AddScoped<IOrderRepository, OrderRepository>()
    .AddScoped<IOrderService, OrderManagementService>();


builder.Services.AddHttpClient<AuthApiClient>();
builder.Services.AddHttpClient<MarketApiClient>();
builder.Services.AddHttpClient<PortfolioApiClient>();


var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();