namespace Ecommerce.Api.Services;

/// <summary>
/// A dummy email service for development that logs emails to the console.
/// Replace this with a real implementation (e.g., SendGrid, Mailgun) in production.
/// </summary>
public class DummyEmailService : IEmailService
{
    private readonly ILogger<DummyEmailService> _logger;

    public DummyEmailService(ILogger<DummyEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("---- Sending Email ----");
        _logger.LogInformation("To: {To}", to);
        _logger.LogInformation("Subject: {Subject}", subject);
        _logger.LogInformation("Body: {Body}", body);
        _logger.LogInformation("-----------------------");
        return Task.CompletedTask;
    }
}