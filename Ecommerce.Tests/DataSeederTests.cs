using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ecommerce.Tests;

public class DataSeederTests
{
    [Fact]
    public async Task SeedAsync_PopulatesProductsAndSales_WhenDatabaseIsEmpty()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        await DataSeeder.SeedAsync(context, NullLoggerFactory.Instance.CreateLogger<DataSeeder>());

        (await context.Products.CountAsync()).Should().BeGreaterThan(0);
        (await context.Sales.CountAsync()).Should().BeGreaterThan(0);
        (await context.SaleItems.CountAsync()).Should().BeGreaterThan(0);
    }
}
