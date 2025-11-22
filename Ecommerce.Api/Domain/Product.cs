using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Api.Domain;

/// <summary>
/// Represents a product in the e-commerce system
/// </summary>
public class Product : Entity
{
    /// <summary>
    /// The name of the product
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the product
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Current price of the product
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// SKU (Stock Keeping Unit) of the product
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Stock quantity available
    /// </summary>
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; } = 0;

    /// <summary>
    /// Public image URL hosted in Cloudinary (or other CDN)
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Indicates if the product is active and available for sale
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Foreign key for the category
    /// </summary>
    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Navigation property for the category
    /// </summary>
    public virtual Category Category { get; set; } = null!;

    /// <summary>
    /// Navigation property for price history
    /// </summary>
    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

    /// <summary>
    /// Navigation property for sale items
    /// </summary>
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    /// <summary>
    /// Creates a new price history entry when price changes
    /// </summary>
    /// <param name="oldPrice">The previous price</param>
    /// <param name="newPrice">The new price</param>
    public void UpdatePrice(decimal oldPrice, decimal newPrice)
    {
        if (oldPrice != newPrice)
        {
            PriceHistories.Add(new PriceHistory
            {
                ProductId = Id,
                OldPrice = oldPrice,
                NewPrice = newPrice,
                ChangedAt = DateTime.UtcNow
            });
            Price = newPrice;
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Updates stock quantity
    /// </summary>
    /// <param name="quantity">The new stock quantity</param>
    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

        StockQuantity = quantity;
        UpdateTimestamp();
    }

    /// <summary>
    /// Reduces stock by the specified amount
    /// </summary>
    /// <param name="quantity">Amount to reduce</param>
    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (StockQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock available");

        StockQuantity -= quantity;
        UpdateTimestamp();
    }

    /// <summary>
    /// Validates the product data
    /// </summary>
    /// <returns>True if the product is valid</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && 
               Name.Length >= 2 && 
               Name.Length <= 200 &&
               !string.IsNullOrWhiteSpace(Sku) &&
               Price > 0 &&
               StockQuantity >= 0;
    }
}
