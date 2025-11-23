using Ecommerce.Api;
using Ecommerce.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ecommerce.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContextOptions/AppDbContext registrations (Postgres)
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(AppDbContext));

            // Remove any Npgsql provider registrations to avoid dual-provider conflicts
            var npgsqlDescriptors = services
                .Where(d =>
                    d.ImplementationType?.Namespace?.Contains("Npgsql.EntityFrameworkCore.PostgreSQL") == true ||
                    d.ServiceType.Namespace?.Contains("Npgsql.EntityFrameworkCore.PostgreSQL") == true)
                .ToList();
            foreach (var d in npgsqlDescriptors)
            {
                services.Remove(d);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase($"EcommerceTests-{Guid.NewGuid()}");
            });
        });
    }
}
