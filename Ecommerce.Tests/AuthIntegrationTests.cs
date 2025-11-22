using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Tests;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Then_Login_After_EmailConfirm_Succeeds()
    {
        var client = _factory.CreateClient();

        var registerPayload = new RegisterDto("Test", "User", "1234567890", "testuser@example.com", "Test123!");
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerPayload);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        // Confirm email manually via UserManager
        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(registerPayload.Email);
            Assert.NotNull(user);
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
            var confirmResult = await userManager.ConfirmEmailAsync(user!, token);
            Assert.True(confirmResult.Succeeded);
        }

        var loginPayload = new LoginDto(registerPayload.Email, registerPayload.Password);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginPayload);
        loginResponse.EnsureSuccessStatusCode();
    }
}
