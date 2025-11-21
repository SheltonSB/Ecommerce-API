using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Api.Services;

/// <summary>
/// Simple SMTP email sender. Configure via EmailSettings in appsettings or environment variables.
/// </summary>
public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSender>? _logger;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender>? logger = null)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var host = _configuration["EmailSettings:Host"];
        var portString = _configuration["EmailSettings:Port"];
        var from = _configuration["EmailSettings:From"];
        var user = _configuration["EmailSettings:UserName"];
        var password = _configuration["EmailSettings:Password"];
        var enableSsl = _configuration.GetValue<bool?>("EmailSettings:EnableSsl") ?? true;

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
        {
            _logger?.LogWarning("Email settings missing. Skipping email send for subject {Subject} to {ToEmail}", subject, toEmail);
            return;
        }

        var port = 587;
        if (!string.IsNullOrWhiteSpace(portString) && int.TryParse(portString, out var parsedPort))
        {
            port = parsedPort;
        }

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl
        };

        if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(password))
        {
            client.Credentials = new NetworkCredential(user, password);
        }

        var mailMessage = new MailMessage
        {
            From = new MailAddress(from),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            // swallow in dev so registration/login flows don't break when email is unconfigured
        }
    }
}
