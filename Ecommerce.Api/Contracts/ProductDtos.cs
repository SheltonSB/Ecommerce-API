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
    /// Stock quantity available
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; } = 0;

    /// <summary>
    /// Indicates if the product is active
    /// </summary>
    public bool IsActive { get; set; } = true;

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
    /// Stock quantity available
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }

    /// <summary>
    /// Indicates if the product is active
    /// </summary>
    public bool IsActive { get; set; }

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
