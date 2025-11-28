using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace Ecommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public CheckoutController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        Stripe.StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateSession([FromBody] CheckoutRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            SuccessUrl = $"{_configuration["FrontendURL"]}/success",
            CancelUrl = $"{_configuration["FrontendURL"]}/cart",
            Metadata = new Dictionary<string, string>()
        };

        if (userId != null)
        {
            options.Metadata.Add("UserId", userId);
        }

        var service = new SessionService();
        Session session = await service.CreateAsync(options);
        return Ok(new CheckoutResponseDto(session.Url));
    }
}