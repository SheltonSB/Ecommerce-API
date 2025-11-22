using AutoMapper;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ecommerce.Tests;

public class ProductServicePagingTests
{
    [Fact]
    public async Task GetAllAsync_Returns_Paged_Result_With_TotalCount()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Program)));
        var mapper = config.CreateMapper();
        var cache = new MemoryCache(new MemoryCacheOptions());

        await using var context = new AppDbContext(options);

        context.Categories.Add(new Category { Id = 1, Name = "Cat", CreatedAt = DateTime.UtcNow });
        for (var i = 1; i <= 30; i++)
        {
            context.Products.Add(new Product
            {
                Id = i,
                Name = $"Product {i}",
                Price = i,
                Sku = $"SKU{i}",
                StockQuantity = 10,
                CategoryId = 1,
                CreatedAt = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync();

        var service = new ProductService(context, mapper, NullLogger<ProductService>.Instance, cache);
        var request = new PagedRequest { Page = 2, PageSize = 10 };

        var result = await service.GetAllAsync(request);

        Assert.Equal(10, result.Items.Count());
        Assert.Equal(30, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }
}
