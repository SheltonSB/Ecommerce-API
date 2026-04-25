using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Contracts;
using FluentAssertions;

namespace Ecommerce.Tests;

public class ProductEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProduct_With_NonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = 9999;

        // Act
        var response = await _client.GetAsync($"/api/products/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}