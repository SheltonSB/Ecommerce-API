using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Api.Domain;

public enum ProductStatus
{
    Draft,
    PendingReview,
    Active,
    Blocked
}

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
    /// UPC (Universal Product Code) - Global Barcode
    /// </summary>
    [StringLength(50)]
    public string? Upc { get; set; }

    /// <summary>
    /// GTIN (Global Trade Item Number)
    /// </summary>
    [StringLength(50)]
    public string? Gtin { get; set; }

    /// <summary>
    /// ISBN for books
    /// </summary>
    [StringLength(50)]
    public string? Isbn { get; set; }

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
    /// Key features or bullet points for the product description
    /// </summary>
    public string? KeyFeatures { get; set; }

    /// <summary>
    /// Current status of the product workflow
    /// </summary>
    public ProductStatus Status { get; set; } = ProductStatus.Draft;

    /// <summary>
    /// Physical location of the inventory (e.g., "Warehouse", "Store")
    /// </summary>
    [StringLength(100)]
    public string? InventoryLocation { get; set; }

    /// <summary>
    /// Weight in pounds (lbs)
    /// </summary>
    public double Weight { get; set; }

    /// <summary>
    /// Height in inches
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Width in inches
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Length in inches
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// Indicates if the product is hazardous material
    /// </summary>
    public bool IsHazmat { get; set; }

    /// <summary>
    /// URL to the Safety Data Sheet (required if IsHazmat is true)
    /// </summary>
    public string? SafetyDataSheetUrl { get; set; }

    /// <summary>
    /// Lowest allowed price for automated repricing
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? FloorPrice { get; set; }

    /// <summary>
    /// Highest allowed price for automated repricing
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? CeilingPrice { get; set; }

    /// <summary>
    /// Calculated quality score (0-100) based on listing completeness
    /// </summary>
    public int ListingQualityScore { get; private set; }

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

    /// <summary>
    /// Updates the listing quality score based on content completeness
    /// </summary>
    public void UpdateQualityScore()
    {
        int score = 0;

        if (Name.Length >= 50) score += 20; // Title length
        if (!string.IsNullOrWhiteSpace(Description) && Description.Length > 100) score += 20; // Description depth
        if (!string.IsNullOrWhiteSpace(KeyFeatures)) score += 20; // Key features present
        if (!string.IsNullOrWhiteSpace(ImageUrl)) score += 20; // Has image
        if (!string.IsNullOrWhiteSpace(Upc) || !string.IsNullOrWhiteSpace(Gtin) || !string.IsNullOrWhiteSpace(Isbn)) score += 20; // Has global ID

        ListingQualityScore = score;
    }

    /// <summary>
    /// Runs compliance checks for restricted keywords and hazmat requirements
    /// </summary>
    public void RunComplianceCheck()
    {
        // Restricted Keywords Check
        var restrictedKeywords = new[] { "Cures Cancer", "FDA Approved", "Miracle Cure" };
        foreach (var keyword in restrictedKeywords)
        {
            if (Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                (Description != null && Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            {
                Status = ProductStatus.Blocked;
                return;
            }
        }

        // Hazmat Check
        if (IsHazmat && string.IsNullOrWhiteSpace(SafetyDataSheetUrl))
        {
            Status = ProductStatus.Blocked; // Blocked until SDS is provided
            return;
        }

        // If currently blocked but passes checks, move to PendingReview (requires human approval) or Active
        if (Status == ProductStatus.Blocked)
        {
            Status = ProductStatus.PendingReview;
        }
    }

    /// <summary>
    /// Calculates estimated fulfillment fee based on dimensions and weight
    /// </summary>
    public decimal CalculateEstimatedFulfillmentFee()
    {
        // Simplified logic: Base fee + weight surcharge + oversize surcharge
        decimal baseFee = 5.00m;
        
        // Dimensional weight calculation (Length x Width x Height / 139)
        double dimWeight = (Length * Width * Height) / 139.0;
        double billableWeight = Math.Max(Weight, dimWeight);

        if (billableWeight > 1.0)
        {
            baseFee += (decimal)((billableWeight - 1.0) * 0.50); // $0.50 per additional lb
        }

        return baseFee;
    }
}
