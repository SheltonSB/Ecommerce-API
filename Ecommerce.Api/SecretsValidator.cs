namespace Ecommerce.Api;

/// <summary>
/// Validates that critical secrets are present in the configuration on application startup.
/// </summary>
public static class SecretsValidator
{
    public static void Validate(IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("CRITICAL ERROR: JWT Key ('Jwt:Key') is not configured. Application cannot start.");
        }

        var stripeKey = configuration["Stripe:SecretKey"];
        if (string.IsNullOrEmpty(stripeKey))
        {
            throw new InvalidOperationException("CRITICAL ERROR: Stripe Secret Key ('Stripe:SecretKey') is not configured. Application cannot start.");
        }

        var stripePublishableKey = configuration["Stripe:PublishableKey"];
        if (string.IsNullOrEmpty(stripePublishableKey))
        {
            throw new InvalidOperationException("CRITICAL ERROR: Stripe Publishable Key ('Stripe:PublishableKey') is not configured. Application cannot start.");
        }
    }
}
