using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Domain;
using Microsoft.AspNetCore.Identity;
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

        // Seed admin
        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            const string role = "Admin";
            const string email = "admin@test.com";
            const string password = "Test123!";
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
            var admin = await userManager.FindByEmailAsync(email);
            if (admin == null)
            {
                admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                await userManager.CreateAsync(admin, password);
            }
            if (!await userManager.IsInRoleAsync(admin, role))
            {
                await userManager.AddToRoleAsync(admin, role);
            }
        }

        // login to get token
        var login = await client.PostAsJsonAsync("/api/auth/login", new LoginDto("admin@test.com", "Test123!"));
        login.EnsureSuccessStatusCode();
        var token = (await login.Content.ReadFromJsonAsync<AuthResponseDto>())?.Token;
        Assert.False(string.IsNullOrWhiteSpace(token));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // create category to reference
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
            db.Categories.Add(new Category { Name = "Cat", CreatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();
        }

        var form = new MultipartFormDataContent();
        form.Add(new StringContent("Test Product"), "Name");
        form.Add(new StringContent("99.99"), "Price");
        form.Add(new StringContent("Test Description"), "Description");
        form.Add(new StringContent("SKU-UPLOAD-1"), "Sku");
        form.Add(new StringContent("5"), "StockQuantity");
        form.Add(new StringContent("1"), "CategoryId");

        var response = await client.PostAsync("/api/admin/products", form);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
