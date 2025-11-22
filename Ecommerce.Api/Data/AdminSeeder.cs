using Ecommerce.Api.Domain;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Api.Data;

/// <summary>
/// Seeds an admin role and user for bootstrapping.
/// </summary>
public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, string adminEmail, string adminPassword)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        const string adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var user = await userManager.FindByEmailAsync(adminEmail);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Admin User"
            };

            var password = string.IsNullOrWhiteSpace(adminPassword) ? Guid.NewGuid().ToString("N") : adminPassword;
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            else if (string.IsNullOrWhiteSpace(adminPassword))
            {
                Console.WriteLine($"Admin user created with generated password: {password}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, adminRole))
        {
            await userManager.AddToRoleAsync(user, adminRole);
        }
    }
}
