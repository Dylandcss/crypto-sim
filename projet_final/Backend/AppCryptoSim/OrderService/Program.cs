using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Extensions;
using OrderService.Data;
using OrderService.Repositories;
using OrderService.Repositories.Interfaces;
using OrderService.Services;
using OrderService.Services.Clients;
using OrderService.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder.AddMicroserviceCore(EnvConstants.OrderServiceUrl)
       .AddMysqlDatabase<OrderDbContext>(EnvConstants.OrderDb);


builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderManagementService>();



builder.Services.AddHttpClient<AuthApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration[EnvConstants.AuthServiceUrl]!));

builder.Services.AddHttpClient<MarketApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration[EnvConstants.MarketServiceUrl]!));

builder.Services.AddHttpClient<PortfolioApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration[EnvConstants.PortfolioServiceUrl]!));

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();