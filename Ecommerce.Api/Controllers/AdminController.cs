using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ecommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
[EnableRateLimiting("authLimiter")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ImageService _imageService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AppDbContext context, ImageService imageService, ILogger<AdminController> logger)
    {
        _context = context;
        _imageService = imageService;
        _logger = logger;
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductWithImageDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        string? imageUrl = null;

        if (dto.Image != null)
        {
            imageUrl = await _imageService.UploadAsync(dto.Image);
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Sku = dto.Sku,
            StockQuantity = dto.StockQuantity,
            CategoryId = dto.CategoryId,
            ImageUrl = imageUrl
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created product {ProductName} with id {ProductId}", product.Name, product.Id);

        return Ok(product);
    }
}
