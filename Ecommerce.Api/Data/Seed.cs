using Ecommerce.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Data;

/// <summary>
/// Database seeding functionality
/// </summary>
public static class Seed
{
    /// <summary>
    /// Initializes the database with seed data
    /// </summary>
    /// <param name="context">Database context</param>
    public static async Task Initialize(AppDbContext context)
    {
        var categorySeed = new[]
        {
            new Category { Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { Name = "Clothing", Description = "Apparel and fashion items" },
            new Category { Name = "Books", Description = "Books and educational materials" },
            new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies" },
            new Category { Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear" },
            new Category { Name = "Beauty & Health", Description = "Beauty products and health supplements" }
        };

        var existingCategoryNames = await context.Categories
            .IgnoreQueryFilters()
            .Select(category => category.Name)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase);

        var categoriesToAdd = categorySeed
            .Where(category => !existingCategoryNames.Contains(category.Name))
            .ToList();

        if (categoriesToAdd.Count > 0)
        {
            await context.Categories.AddRangeAsync(categoriesToAdd);
            await context.SaveChangesAsync();
        }

        var categoryLookup = await context.Categories
            .IgnoreQueryFilters()
            .ToDictionaryAsync(category => category.Name, category => category.Id, StringComparer.OrdinalIgnoreCase);

        var productSeed = new[]
        {
            new Product
            {
                Name = "Smartphone",
                Description = "Latest generation smartphone with advanced features",
                Price = 699.99m,
                Sku = "PHONE-001",
                StockQuantity = 50,
                CategoryId = categoryLookup["Electronics"]
            },
            new Product
            {
                Name = "Laptop",
                Description = "High-performance laptop for work and gaming",
                Price = 1299.99m,
                Sku = "LAPTOP-001",
                StockQuantity = 25,
                CategoryId = categoryLookup["Electronics"]
            },
            new Product
            {
                Name = "Headphones",
                Description = "Wireless noise-canceling headphones",
                Price = 299.99m,
                Sku = "HEADPHONE-001",
                StockQuantity = 75,
                CategoryId = categoryLookup["Electronics"]
            },
            new Product
            {
                Name = "Smart Watch",
                Description = "Fitness tracking smartwatch with health monitoring",
                Price = 249.99m,
                Sku = "WATCH-001",
                StockQuantity = 40,
                CategoryId = categoryLookup["Electronics"]
            },
            new Product
            {
                Name = "T-Shirt",
                Description = "Comfortable cotton t-shirt",
                Price = 19.99m,
                Sku = "TSHIRT-001",
                StockQuantity = 100,
                CategoryId = categoryLookup["Clothing"]
            },
            new Product
            {
                Name = "Jeans",
                Description = "Classic denim jeans",
                Price = 79.99m,
                Sku = "JEANS-001",
                StockQuantity = 60,
                CategoryId = categoryLookup["Clothing"]
            },
            new Product
            {
                Name = "Sneakers",
                Description = "Comfortable running sneakers",
                Price = 119.99m,
                Sku = "SNEAKERS-001",
                StockQuantity = 80,
                CategoryId = categoryLookup["Clothing"]
            },
            new Product
            {
                Name = "Programming Book",
                Description = "Comprehensive guide to software development",
                Price = 49.99m,
                Sku = "BOOK-001",
                StockQuantity = 75,
                CategoryId = categoryLookup["Books"]
            },
            new Product
            {
                Name = "Business Strategy",
                Description = "Guide to business strategy and management",
                Price = 34.99m,
                Sku = "BOOK-002",
                StockQuantity = 50,
                CategoryId = categoryLookup["Books"]
            },
            new Product
            {
                Name = "Cookbook",
                Description = "Collection of delicious recipes",
                Price = 24.99m,
                Sku = "BOOK-003",
                StockQuantity = 90,
                CategoryId = categoryLookup["Books"]
            },
            new Product
            {
                Name = "Garden Tools Set",
                Description = "Complete set of gardening tools",
                Price = 89.99m,
                Sku = "GARDEN-001",
                StockQuantity = 30,
                CategoryId = categoryLookup["Home & Garden"]
            },
            new Product
            {
                Name = "LED Light Bulbs",
                Description = "Energy-efficient LED light bulbs (pack of 6)",
                Price = 29.99m,
                Sku = "LIGHT-001",
                StockQuantity = 200,
                CategoryId = categoryLookup["Home & Garden"]
            },
            new Product
            {
                Name = "Yoga Mat",
                Description = "Premium non-slip yoga mat",
                Price = 39.99m,
                Sku = "YOGA-001",
                StockQuantity = 65,
                CategoryId = categoryLookup["Sports & Outdoors"]
            },
            new Product
            {
                Name = "Water Bottle",
                Description = "Insulated stainless steel water bottle",
                Price = 24.99m,
                Sku = "BOTTLE-001",
                StockQuantity = 120,
                CategoryId = categoryLookup["Sports & Outdoors"]
            },
            new Product
            {
                Name = "Skincare Set",
                Description = "Complete skincare routine set",
                Price = 79.99m,
                Sku = "SKINCARE-001",
                StockQuantity = 45,
                CategoryId = categoryLookup["Beauty & Health"]
            },
            new Product
            {
                Name = "Vitamin Supplements",
                Description = "Daily multivitamin supplements (30-day supply)",
                Price = 19.99m,
                Sku = "VITAMIN-001",
                StockQuantity = 150,
                CategoryId = categoryLookup["Beauty & Health"]
            }
        };

        var existingSkus = await context.Products
            .IgnoreQueryFilters()
            .Select(product => product.Sku)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase);

        var productsToAdd = productSeed
            .Where(product => !existingSkus.Contains(product.Sku))
            .ToList();

        if (productsToAdd.Count > 0)
        {
            await context.Products.AddRangeAsync(productsToAdd);
            await context.SaveChangesAsync();
        }

        var products = await context.Products
            .IgnoreQueryFilters()
            .ToListAsync();

        if (!await context.Sales.IgnoreQueryFilters().AnyAsync())
        {
            var sales = new List<Sale>();
            var random = new Random();

            for (var i = 1; i <= 20; i++)
            {
                var seededStatus = (SaleStatus)random.Next(0, 4);
                var sale = new Sale
                {
                    SaleNumber = $"SALE-{i:D6}",
                    SaleDate = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                    CustomerName = $"Customer {i}",
                    CustomerEmail = $"customer{i}@example.com",
                    Status = SaleStatus.Pending,
                    TaxAmount = 0,
                    DiscountAmount = random.Next(0, 2) == 1 ? random.Next(5, 25) : 0,
                    Notes = random.Next(0, 2) == 1 ? $"Special order notes for sale {i}" : null
                };

                var itemCount = random.Next(1, 4);
                var selectedProducts = products.OrderBy(_ => random.Next()).Take(itemCount);

                foreach (var product in selectedProducts)
                {
                    sale.AddSaleItem(product, random.Next(1, 4));
                }

                sale.CalculateTotalAmount();
                sale.Status = seededStatus;

                if (seededStatus == SaleStatus.Completed)
                {
                    sale.PaymentInfo = new PaymentInfo
                    {
                        PaymentMethod = (PaymentMethod)random.Next(0, 7),
                        Amount = sale.FinalAmount,
                        Status = PaymentStatus.Completed,
                        TransactionId = $"TXN-{random.Next(100000, 999999)}",
                        ProcessedAt = sale.SaleDate.AddMinutes(random.Next(1, 60))
                    };
                }

                sales.Add(sale);
            }

            await context.Sales.AddRangeAsync(sales);
            await context.SaveChangesAsync();
        }

        if (!await context.PriceHistories.IgnoreQueryFilters().AnyAsync())
        {
            var random = new Random();
            var priceHistories = new List<PriceHistory>();

            foreach (var product in products.Take(5))
            {
                var oldPrice = product.Price;
                var newPrice = Math.Round(oldPrice * (decimal)(0.8 + (random.NextDouble() * 0.4)), 2);

                priceHistories.Add(new PriceHistory
                {
                    ProductId = product.Id,
                    OldPrice = oldPrice,
                    NewPrice = newPrice,
                    ChangedAt = DateTime.UtcNow.AddDays(-random.Next(1, 100))
                });
            }

            await context.PriceHistories.AddRangeAsync(priceHistories);
            await context.SaveChangesAsync();
        }
    }
}
