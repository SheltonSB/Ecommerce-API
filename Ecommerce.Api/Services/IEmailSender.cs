using System.Threading.Tasks;

namespace Ecommerce.Api.Services;

/// <summary>
/// Contract for sending emails (e.g., verification, password reset).
/// </summary>
public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
}
