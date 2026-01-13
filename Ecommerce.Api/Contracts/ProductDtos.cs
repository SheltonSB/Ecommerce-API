using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Contracts;

/// <summary>
/// DTO for creating a new product
/// </summary>
public class CreateProductDto
{
    /// <summary>
    /// The name of the product
    /// </summary>
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the product
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Price of the product
    /// </summary>
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// SKU (Stock Keeping Unit) of the product
    /// </summary>
    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU must be between 3 and 50 characters")]
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// UPC (Universal Product Code) - Global Barcode
    /// </summary>
    [StringLength(50, ErrorMessage = "UPC cannot exceed 50 characters")]
    public string? Upc { get; set; }

    /// <summary>
    /// GTIN (Global Trade Item Number)
    /// </summary>
    [StringLength(50, ErrorMessage = "GTIN cannot exceed 50 characters")]
    public string? Gtin { get; set; }

    /// <summary>
    /// ISBN for books
    /// </summary>
    [StringLength(50, ErrorMessage = "ISBN cannot exceed 50 characters")]
    public string? Isbn { get; set; }

    /// <summary>
    /// Stock quantity available
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; } = 0;

    /// <summary>
    /// Key features or bullet points for the product description
    /// </summary>
    public string? KeyFeatures { get; set; }

    /// <summary>
    /// Indicates if the product is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Physical location of the inventory (e.g., "Warehouse", "Store")
    /// </summary>
    [StringLength(100, ErrorMessage = "Inventory location cannot exceed 100 characters")]
    public string? InventoryLocation { get; set; }

    /// <summary>
    /// Weight in pounds (lbs)
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Weight cannot be negative")]
    public double Weight { get; set; }

    /// <summary>
    /// Height in inches
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Height cannot be negative")]
    public double Height { get; set; }

    /// <summary>
    /// Width in inches
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Width cannot be negative")]
    public double Width { get; set; }

    /// <summary>
    /// Length in inches
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Length cannot be negative")]
    public double Length { get; set; }

    /// <summary>
    /// Indicates if the product is hazardous material
    /// </summary>
    public bool IsHazmat { get; set; }

    /// <summary>
    /// URL to the Safety Data Sheet (required if IsHazmat is true)
    /// </summary>
    [Url(ErrorMessage = "Safety data sheet URL must be a valid URL")]
    public string? SafetyDataSheetUrl { get; set; }

    /// <summary>
    /// Lowest allowed price for automated repricing
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Floor price cannot be negative")]
    public decimal? FloorPrice { get; set; }

    /// <summary>
    /// Highest allowed price for automated repricing
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Ceiling price cannot be negative")]
    public decimal? CeilingPrice { get; set; }

    /// <summary>
    /// Category ID for the product
    /// </summary>
    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }
}

/// <summary>
/// DTO for updating an existing product
/// </summary>
public class UpdateProductDto
{
    /// <summary>
    /// The name of the product
    /// </summary>
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the product
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// SKU (Stock Keeping Unit) of the product
    /// </summary>
    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU must be between 3 and 50 characters")]
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// UPC (Universal Product Code) - Global Barcode
    /// </summary>
    [StringLength(50, ErrorMessage = "UPC cannot exceed 50 characters")]
    public string? Upc { get; set; }

    /// <summary>
    /// GTIN (Global Trade Item Number)
    /// </summary>
    [StringLength(50, ErrorMessage = "GTIN cannot exceed 50 characters")]
    public string? Gtin { get; set; }

    /// <summary>
    /// ISBN for books
    /// </summary>
    [StringLength(50, ErrorMessage = "ISBN cannot exceed 50 characters")]
    public string? Isbn { get; set; }

    /// <summary>
    /// Stock quantity available
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }

    /// <summary>
    /// Key features or bullet points for the product description
    /// </summary>
    public string? KeyFeatures { get; set; }

    /// <summary>
    /// Indicates if the product is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Physical location of the inventory (e.g., "Warehouse", "Store")
    /// </summary>
    [StringLength(100, ErrorMessage = "Inventory location cannot exceed 100 characters")]
    public string? InventoryLocation { get; set; }

    /// <summary>
    /// Weight in pounds (lbs)
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Weight cannot be negative")]
    public double Weight { get; set; }

    /// <summary>
    /// Height in inches
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Height cannot be negative")]
    public double Height { get; set; }

    /// <summary>
    /// Width in inches
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Width cannot be negative")]
    public double Width { get; set; }

    /// <summary>
    /// Length in inches
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Length cannot be negative")]
    public double Length { get; set; }

    /// <summary>
    /// Indicates if the product is hazardous material
    /// </summary>
    public bool IsHazmat { get; set; }

    /// <summary>
    /// URL to the Safety Data Sheet (required if IsHazmat is true)
    /// </summary>
    [Url(ErrorMessage = "Safety data sheet URL must be a valid URL")]
    public string? SafetyDataSheetUrl { get; set; }

    /// <summary>
    /// Lowest allowed price for automated repricing
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Floor price cannot be negative")]
    public decimal? FloorPrice { get; set; }

    /// <summary>
    /// Highest allowed price for automated repricing
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Ceiling price cannot be negative")]
    public decimal? CeilingPrice { get; set; }

    /// <summary>
    /// Category ID for the product
    /// </summary>
    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }
}

/// <summary>
/// DTO for updating product stock
/// </summary>
public class UpdateStockDto
{
    /// <summary>
    /// New stock quantity
    /// </summary>
    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }
}

/// <summary>
/// DTO for returning product information
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the product
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Current price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// SKU (Stock Keeping Unit) of the product
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Stock quantity available
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Public image URL for the product
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Indicates if the product is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Category information
    /// </summary>
    public CategoryListItemDto Category { get; set; } = null!;

    /// <summary>
    /// Timestamp when the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the product was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for product list items
/// </summary>
public class ProductListItemDto
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Current price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// SKU (Stock Keeping Unit) of the product
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Stock quantity available
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Public image URL for the product
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Indicates if the product is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;
}

/// <summary>
/// DTO for price history entries
/// </summary>
public class PriceHistoryDto
{
    /// <summary>
    /// Unique identifier for the price history entry
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The previous price
    /// </summary>
    public decimal OldPrice { get; set; }

    /// <summary>
    /// The new price
    /// </summary>
    public decimal NewPrice { get; set; }

    /// <summary>
    /// When the price change occurred
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// Percentage change in price
    /// </summary>
    public decimal PercentageChange { get; set; }

    /// <summary>
    /// Indicates if this was a price increase
    /// </summary>
    public bool IsPriceIncrease { get; set; }
}
