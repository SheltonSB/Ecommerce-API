using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Tests;

public class AdminUploadTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AdminUploadTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Admin_Can_Create_Product_With_Image()
    {
        var client = _factory.CreateClient();

        // login to get token
        var login = await client.PostAsJsonAsync("/api/auth/login", new LoginDto("admin@ecommerce.com", "ChangeMe_AdminPassword1!"));
        login.EnsureSuccessStatusCode();
        var token = (await login.Content.ReadFromJsonAsync<AuthResponseDto>())?.Token;
        Assert.False(string.IsNullOrWhiteSpace(token));
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // create category to reference
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Categories.Add(new Category { Name = "Cat", CreatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();
        }

        var response = await client.PostAsJsonAsync("/api/admin/products", new CreateProductDto
        {
            Name = "Test Product",
            Price = 99.99m,
            Description = "Test Description",
            Sku = "SKU-UPLOAD-1",
            StockQuantity = 5,
            CategoryId = 1,
            ImageUrl = "https://cdn.example.com/test-product.png"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
