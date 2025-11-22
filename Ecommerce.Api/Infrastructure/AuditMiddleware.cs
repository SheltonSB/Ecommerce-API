using System.Security.Claims;

namespace Ecommerce.Api.Infrastructure;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method != HttpMethods.Get && context.Request.Method != HttpMethods.Options)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";
            var path = context.Request.Path;
            var method = context.Request.Method;
            _logger.LogInformation("AUDIT: User {UserId} performed {Method} on {Path}", userId, method, path);
        }

        await _next(context);
    }
}
