using Ecommerce.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Data;

/// <summary>
/// Handles seeding the database with initial data.
/// This simulates loading a dataset from a source like Kaggle.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Products.AnyAsync())
        {
            logger.LogInformation("Database already contains data. Skipping seeding.");
            return;
        }

        logger.LogInformation("Starting database seeding...");

        // Seed Categories
        var categories = new List<Category>
        {
            new() { Name = "Electronics", Description = "Gadgets and devices" },
            new() { Name = "Books", Description = "Printed and digital books" },
            new() { Name = "Apparel", Description = "Clothing and accessories" }
        };
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Seed Products
        var products = new List<Product>
        {
            new() { Name = "Laptop Pro", Price = 1200, Sku = "LP-001", StockQuantity = 50, CategoryId = categories[0].Id },
            new() { Name = "Smartphone X", Price = 800, Sku = "SP-002", StockQuantity = 150, CategoryId = categories[0].Id },
            new() { Name = "The Great Novel", Price = 25, Sku = "BK-003", StockQuantity = 300, CategoryId = categories[1].Id },
            new() { Name = "Classic T-Shirt", Price = 30, Sku = "AP-004", StockQuantity = 500, CategoryId = categories[2].Id },
            new() { Name = "Wireless Headphones", Price = 150, Sku = "WH-005", StockQuantity = 200, CategoryId = categories[0].Id }
        };
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // Seed Sales Data (simulating a Kaggle dataset)
        var random = new Random();
        var sales = new List<Sale>();
        var locations = new[]
        {
            ("New York", "NY", "USA"),
            ("Los Angeles", "CA", "USA"),
            ("London", null, "UK"),
            ("Tokyo", null, "Japan"),
            ("Sydney", "NSW", "Australia")
        };

        for (int i = 0; i < 100; i++)
        {
            var saleDate = DateTime.UtcNow.AddDays(-random.Next(1, 30));
            var (city, state, country) = locations[random.Next(locations.Length)];

            var sale = new Sale
            {
                SaleNumber = $"SALE-{saleDate:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
                SaleDate = saleDate,
                Status = SaleStatus.Completed,
                CustomerName = $"Customer {i + 1}",
                CustomerEmail = $"customer{i + 1}@example.com",
                CustomerCity = city,
                CustomerState = state,
                CustomerCountry = country
            };

            var numberOfItems = random.Next(1, 4);
            for (int j = 0; j < numberOfItems; j++)
            {
                var product = products[random.Next(products.Count)];
                var quantity = random.Next(1, 5);

                // Check if item already exists to avoid duplicates
                if (!sale.SaleItems.Any(si => si.ProductId == product.Id))
                {
                    sale.AddSaleItem(product, quantity);
                }
            }

            sales.Add(sale);
        }

        await context.Sales.AddRangeAsync(sales);
        await context.SaveChangesAsync();

        logger.LogInformation("Database seeding completed successfully.");
    }
}