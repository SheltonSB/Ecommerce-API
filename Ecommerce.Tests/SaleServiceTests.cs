using AutoMapper;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Mappings;
using Ecommerce.Api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ecommerce.Tests;

public class SaleServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
        return config.CreateMapper();
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Stock_Decrements_Only_On_Completion()
    {
        using var context = CreateContext();
        var category = new Category { Name = "TestCat" };
        var product = new Product { Name = "Test", Price = 10m, Sku = "SKU-1", StockQuantity = 5, Category = category };
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new SaleService(context, CreateMapper(), NullLogger<SaleService>.Instance);

        var dto = new CreateSaleDto
        {
            CustomerName = "Buyer",
            CustomerEmail = "buyer@example.com",
            TaxAmount = 0,
            DiscountAmount = 0,
            Notes = null,
            SaleItems = new List<CreateSaleItemDto> { new() { ProductId = product.Id, Quantity = 2 } }
        };

        var sale = await service.CreateAsync(dto);

        (await context.Products.FindAsync(product.Id))!.StockQuantity.Should().Be(5);

        var completed = await service.CompleteAsync(sale.Id);
        completed.Should().BeTrue();
        (await context.Products.FindAsync(product.Id))!.StockQuantity.Should().Be(3);
    }
}
