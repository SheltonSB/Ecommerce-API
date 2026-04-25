using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;
using Polly;

namespace Ecommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IAsyncPolicy _resiliencePolicy;

    public CheckoutController(AppDbContext context, IConfiguration configuration, IAsyncPolicy resiliencePolicy)
    {
        _context = context;
        _configuration = configuration;
        _resiliencePolicy = resiliencePolicy;
        Stripe.StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateSession([FromBody] CheckoutRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var publishableKey = _configuration["Stripe:PublishableKey"];
        if (string.IsNullOrWhiteSpace(publishableKey))
        {
            return StatusCode(500, "Stripe publishable key is not configured.");
        }

        var frontendBaseUrl = _configuration["FrontendURL"];
        if (string.IsNullOrWhiteSpace(frontendBaseUrl) ||
            frontendBaseUrl.Contains("your-production-domain.com", StringComparison.OrdinalIgnoreCase))
        {
            frontendBaseUrl = Request.Headers.Origin.FirstOrDefault() ?? $"{Request.Scheme}://{Request.Host}";
        }
        frontendBaseUrl = frontendBaseUrl.TrimEnd('/');

        var successUrl = _configuration["StripeSuccessUrl"];
        if (string.IsNullOrWhiteSpace(successUrl) ||
            successUrl.Contains("your-production-domain.com", StringComparison.OrdinalIgnoreCase))
        {
            successUrl = $"{frontendBaseUrl}/success";
        }

        var cancelUrl = _configuration["StripeCancelUrl"];
        if (string.IsNullOrWhiteSpace(cancelUrl) ||
            cancelUrl.Contains("your-production-domain.com", StringComparison.OrdinalIgnoreCase))
        {
            cancelUrl = $"{frontendBaseUrl}/cart";
        }

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

        var lineItems = new List<SessionLineItemOptions>();
        foreach (var item in request.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null) continue;

            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(product.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions { Name = product.Name }
                },
                Quantity = item.Quantity,
            });
        }

        var options = new SessionCreateOptions
        {
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string>()
        };

        if (userId != null)
        {
            options.Metadata.Add("UserId", userId);
        }

        var service = new SessionService();
        Session session = await _resiliencePolicy.ExecuteAsync(() => service.CreateAsync(options));
        return Ok(new CheckoutResponseDto(session.Url ?? string.Empty, session.Id, publishableKey));
    }
}
