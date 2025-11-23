using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Ecommerce.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var errorResponse = new { message = exception.Message, details = (string?)null };

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new { message = "Validation failed", details = validationException.Errors.Select(e => e.ErrorMessage).FirstOrDefault() };
                break;
            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError(exception, "An unhandled exception has occurred.");
                break;
        }

        var result = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(result);
    }
}