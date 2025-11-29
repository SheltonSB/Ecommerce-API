using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Contracts;
using FluentAssertions;

namespace Ecommerce.Tests;

public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthorizationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProductWrites_WithoutToken_AreRejected()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/products", new CreateProductDto
        {
            Name = "Locked",
            Sku = "LOCK-1",
            Price = 10,
            StockQuantity = 1,
            CategoryId = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
