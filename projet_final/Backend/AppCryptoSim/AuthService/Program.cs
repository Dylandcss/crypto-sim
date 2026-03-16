using AuthService.Data;
using AuthService.Services;
using AuthService.Repositories;
using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.AddMicroserviceCore(EnvConstants.AuthServiceUrl)
       .AddMysqlDatabase<AuthDbContext>(EnvConstants.AuthDb);

builder.Services
    .AddScoped<IAuthRepository, AuthRepository>()
    .AddScoped<IAuthManagementService, AuthManagementService>()
    .AddSingleton<JwtTokenService>();

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();