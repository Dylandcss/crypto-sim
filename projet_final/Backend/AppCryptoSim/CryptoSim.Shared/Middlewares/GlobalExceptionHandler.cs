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
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception capturée par le middleware");
            await HandleExceptionAsync(context, ex);

        }

    }


    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            ExceptionBase customException => (customException.StatusCode, customException.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "Une erreur interne du serveur s'est produite.")
        };

        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(new { message, statusCode });

    }


}
