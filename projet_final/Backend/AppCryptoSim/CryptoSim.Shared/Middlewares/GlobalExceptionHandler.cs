using CryptoSim.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;


namespace CryptoSim.Shared.Middlewares;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context);}
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception capturée par le middleware");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = ex switch {
            ExceptionBase customException => new ErrorResponse(customException.Message, customException.StatusCode),
            _ => new ErrorResponse("Une erreur interne du serveur s'est produite.", (int) HttpStatusCode.InternalServerError, ex.Message)
        };

        context.Response.StatusCode = response.StatusCode;
        return context.Response.WriteAsJsonAsync(response);
    }

    private record ErrorResponse(string Message, int StatusCode, string Details = "");

}
