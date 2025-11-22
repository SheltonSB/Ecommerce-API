using System.Net;
using Ecommerce.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Ecommerce.Tests;

public class ApiHealthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiHealthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Products_Endpoint_Returns_Success()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
