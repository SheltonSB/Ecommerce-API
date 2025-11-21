using Ecommerce.Api.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }
    public DbSet<PaymentInfo> PaymentInfos { get; set; }
    public DbSet<UserInteraction> UserInteractions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SaleItem>()
            .HasOne(si => si.Product)
            .WithMany()
            .HasForeignKey(si => si.ProductId);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Sale>()
            .Property(s => s.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<PriceHistory>()
            .Property(ph => ph.NewPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<PaymentInfo>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");
    }
}
