using Ecommerce.Api.Domain;

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
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (context.Categories.Any())
        {
            return; // Database has been seeded
        }

        // Seed categories
        var categories = new[]
        {
            new Category { Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { Name = "Clothing", Description = "Apparel and fashion items" },
            new Category { Name = "Books", Description = "Books and educational materials" },
            new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies" },
            new Category { Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear" },
            new Category { Name = "Beauty & Health", Description = "Beauty products and health supplements" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Seed products
        var products = new[]
        {
            // Electronics
            new Product 
            { 
                Name = "Smartphone", 
                Description = "Latest generation smartphone with advanced features", 
                Price = 699.99m, 
                Sku = "PHONE-001", 
                StockQuantity = 50, 
                CategoryId = 1 
            },
            new Product 
            { 
                Name = "Laptop", 
                Description = "High-performance laptop for work and gaming", 
                Price = 1299.99m, 
                Sku = "LAPTOP-001", 
                StockQuantity = 25, 
                CategoryId = 1 
            },
            new Product 
            { 
                Name = "Headphones", 
                Description = "Wireless noise-canceling headphones", 
                Price = 299.99m, 
                Sku = "HEADPHONE-001", 
                StockQuantity = 75, 
                CategoryId = 1 
            },
            new Product 
            { 
                Name = "Smart Watch", 
                Description = "Fitness tracking smartwatch with health monitoring", 
                Price = 249.99m, 
                Sku = "WATCH-001", 
                StockQuantity = 40, 
                CategoryId = 1 
            },

            // Clothing
            new Product 
            { 
                Name = "T-Shirt", 
                Description = "Comfortable cotton t-shirt", 
                Price = 19.99m, 
                Sku = "TSHIRT-001", 
                StockQuantity = 100, 
                CategoryId = 2 
            },
            new Product 
            { 
                Name = "Jeans", 
                Description = "Classic denim jeans", 
                Price = 79.99m, 
                Sku = "JEANS-001", 
                StockQuantity = 60, 
                CategoryId = 2 
            },
            new Product 
            { 
                Name = "Sneakers", 
                Description = "Comfortable running sneakers", 
                Price = 119.99m, 
                Sku = "SNEAKERS-001", 
                StockQuantity = 80, 
                CategoryId = 2 
            },

            // Books
            new Product 
            { 
                Name = "Programming Book", 
                Description = "Comprehensive guide to software development", 
                Price = 49.99m, 
                Sku = "BOOK-001", 
                StockQuantity = 75, 
                CategoryId = 3 
            },
            new Product 
            { 
                Name = "Business Strategy", 
                Description = "Guide to business strategy and management", 
                Price = 34.99m, 
                Sku = "BOOK-002", 
                StockQuantity = 50, 
                CategoryId = 3 
            },
            new Product 
            { 
                Name = "Cookbook", 
                Description = "Collection of delicious recipes", 
                Price = 24.99m, 
                Sku = "BOOK-003", 
                StockQuantity = 90, 
                CategoryId = 3 
            },

            // Home & Garden
            new Product 
            { 
                Name = "Garden Tools Set", 
                Description = "Complete set of gardening tools", 
                Price = 89.99m, 
                Sku = "GARDEN-001", 
                StockQuantity = 30, 
                CategoryId = 4 
            },
            new Product 
            { 
                Name = "LED Light Bulbs", 
                Description = "Energy-efficient LED light bulbs (pack of 6)", 
                Price = 29.99m, 
                Sku = "LIGHT-001", 
                StockQuantity = 200, 
                CategoryId = 4 
            },

            // Sports & Outdoors
            new Product 
            { 
                Name = "Yoga Mat", 
                Description = "Premium non-slip yoga mat", 
                Price = 39.99m, 
                Sku = "YOGA-001", 
                StockQuantity = 65, 
                CategoryId = 5 
            },
            new Product 
            { 
                Name = "Water Bottle", 
                Description = "Insulated stainless steel water bottle", 
                Price = 24.99m, 
                Sku = "BOTTLE-001", 
                StockQuantity = 120, 
                CategoryId = 5 
            },

            // Beauty & Health
            new Product 
            { 
                Name = "Skincare Set", 
                Description = "Complete skincare routine set", 
                Price = 79.99m, 
                Sku = "SKINCARE-001", 
                StockQuantity = 45, 
                CategoryId = 6 
            },
            new Product 
            { 
                Name = "Vitamin Supplements", 
                Description = "Daily multivitamin supplements (30-day supply)", 
                Price = 19.99m, 
                Sku = "VITAMIN-001", 
                StockQuantity = 150, 
                CategoryId = 6 
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // Generate some sample sales
        var sales = new List<Sale>();
        var random = new Random();

        for (int i = 1; i <= 20; i++)
        {
            var sale = new Sale
            {
                SaleNumber = $"SALE-{i:D6}",
                SaleDate = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                CustomerName = $"Customer {i}",
                CustomerEmail = $"customer{i}@example.com",
                Status = (SaleStatus)random.Next(0, 4),
                TaxAmount = 0,
                DiscountAmount = random.Next(0, 2) == 1 ? random.Next(5, 25) : 0,
                Notes = random.Next(0, 2) == 1 ? $"Special order notes for sale {i}" : null
            };

            // Add random items to each sale
            var itemCount = random.Next(1, 4);
            var selectedProducts = products.OrderBy(x => random.Next()).Take(itemCount);

            foreach (var product in selectedProducts)
            {
                var quantity = random.Next(1, 4);
                sale.AddSaleItem(product, quantity);
            }

            // Calculate totals
            sale.CalculateTotalAmount();

            // Add payment info for completed sales
            if (sale.Status == SaleStatus.Completed)
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

        // Generate some price history
        var priceHistories = new List<PriceHistory>();

        foreach (var product in products.Take(5)) // Only for first 5 products
        {
            var oldPrice = product.Price;
            var newPrice = oldPrice * (decimal)(0.8 + random.NextDouble() * 0.4); // ±20% change
            newPrice = Math.Round(newPrice, 2);

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
