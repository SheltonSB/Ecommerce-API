using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Configurations;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Tests;

public class PostgresContainerFixture : IAsyncLifetime
{
    public PostgreSqlTestcontainer Container { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        Container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "ecommerce",
                Username = "ecommerce",
                Password = "ecommerce_pw"
            })
            .WithImage("postgres:16-alpine")
            .Build();

        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (Container != null)
        {
            await Container.DisposeAsync();
        }
    }
}

public class PostgresContainerTests : IClassFixture<PostgresContainerFixture>
{
    private readonly PostgresContainerFixture _fixture;

    public PostgresContainerTests(PostgresContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Migrations_Apply_And_Can_Persist_Product()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_fixture.Container.ConnectionString)
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.MigrateAsync();

        var product = new Product
        {
            Name = "Container Tee",
            Price = 25,
            Sku = "CONT-001",
            StockQuantity = 10,
            Category = new Category { Name = "Apparel" }
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var persisted = await context.Products.FirstOrDefaultAsync(p => p.Sku == "CONT-001");
        Assert.NotNull(persisted);
        Assert.True(persisted!.Id > 0);
    }
}
