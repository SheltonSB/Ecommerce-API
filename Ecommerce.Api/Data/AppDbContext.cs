using Ecommerce.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Data;

/// <summary>
/// Database context for the E-commerce application
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }
    public DbSet<PaymentInfo> PaymentInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            // Add unique constraint on name
            entity.HasIndex(e => e.Name).IsUnique();
            
            // Configure soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            // Configure relationships
            entity.HasMany(e => e.Products)
                  .WithOne(e => e.Category)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Sku).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
            
            // Add unique constraint on SKU
            entity.HasIndex(e => e.Sku).IsUnique();
            
            // Configure soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            // Configure relationships
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasMany(e => e.PriceHistories)
                  .WithOne(e => e.Product)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasMany(e => e.SaleItems)
                  .WithOne(e => e.Product)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Sale entity
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SaleNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.CustomerEmail).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            // Add unique constraint on sale number
            entity.HasIndex(e => e.SaleNumber).IsUnique();
            
            // Configure soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            // Configure relationships
            entity.HasMany(e => e.SaleItems)
                  .WithOne(e => e.Sale)
                  .HasForeignKey(e => e.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.PaymentInfo)
                  .WithOne(e => e.Sale)
                  .HasForeignKey<PaymentInfo>(e => e.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure SaleItem entity
        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            
            // Configure soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            // Configure relationships
            entity.HasOne(e => e.Sale)
                  .WithMany(e => e.SaleItems)
                  .HasForeignKey(e => e.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Product)
                  .WithMany(e => e.SaleItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure PriceHistory entity
        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OldPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.NewPrice).HasColumnType("decimal(18,2)").IsRequired();
            
            // Configure soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            // Configure relationships
            entity.HasOne(e => e.Product)
                  .WithMany(e => e.PriceHistories)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PaymentInfo entity
        modelBuilder.Entity<PaymentInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.TransactionId).HasMaxLength(100);
            entity.Property(e => e.PaymentReference).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            // Configure soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            // Configure relationships
            entity.HasOne(e => e.Sale)
                  .WithOne(e => e.PaymentInfo)
                  .HasForeignKey<PaymentInfo>(e => e.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates timestamps for entities before saving
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<Entity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdateTimestamp();
                    break;
            }
        }
    }

    /// <summary>
    /// Seeds initial data for the application
    /// </summary>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories", CreatedAt = seedDate },
            new Category { Id = 2, Name = "Clothing", Description = "Apparel and fashion items", CreatedAt = seedDate },
            new Category { Id = 3, Name = "Books", Description = "Books and educational materials", CreatedAt = seedDate },
            new Category { Id = 4, Name = "Home & Garden", Description = "Home improvement and garden supplies", CreatedAt = seedDate }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product 
            { 
                Id = 1, 
                Name = "Smartphone", 
                Description = "Latest generation smartphone with advanced features", 
                Price = 699.99m, 
                Sku = "PHONE-001", 
                StockQuantity = 50, 
                CategoryId = 1,
                CreatedAt = seedDate 
            },
            new Product 
            { 
                Id = 2, 
                Name = "Laptop", 
                Description = "High-performance laptop for work and gaming", 
                Price = 1299.99m, 
                Sku = "LAPTOP-001", 
                StockQuantity = 25, 
                CategoryId = 1,
                CreatedAt = seedDate 
            },
            new Product 
            { 
                Id = 3, 
                Name = "T-Shirt", 
                Description = "Comfortable cotton t-shirt", 
                Price = 19.99m, 
                Sku = "TSHIRT-001", 
                StockQuantity = 100, 
                CategoryId = 2,
                CreatedAt = seedDate 
            },
            new Product 
            { 
                Id = 4, 
                Name = "Programming Book", 
                Description = "Comprehensive guide to software development", 
                Price = 49.99m, 
                Sku = "BOOK-001", 
                StockQuantity = 75, 
                CategoryId = 3,
                CreatedAt = seedDate 
            }
        );
    }
}
