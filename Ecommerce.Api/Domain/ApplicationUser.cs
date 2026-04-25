using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Api.Domain;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? StripeCustomerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
