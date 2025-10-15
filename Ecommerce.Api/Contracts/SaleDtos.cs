using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Contracts;

/// <summary>
/// DTO for creating a new sale
/// </summary>
public class CreateSaleDto
{
    /// <summary>
    /// Customer name
    /// </summary>
    [StringLength(200, ErrorMessage = "Customer name cannot exceed 200 characters")]
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer email
    /// </summary>
    [StringLength(100, ErrorMessage = "Customer email cannot exceed 100 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Notes or comments about the sale
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }

    /// <summary>
    /// Tax amount applied to the sale
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Tax amount cannot be negative")]
    public decimal TaxAmount { get; set; } = 0;

    /// <summary>
    /// Discount amount applied to the sale
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Discount amount cannot be negative")]
    public decimal DiscountAmount { get; set; } = 0;

    /// <summary>
    /// Sale items
    /// </summary>
    [Required(ErrorMessage = "Sale must have at least one item")]
    [MinLength(1, ErrorMessage = "Sale must have at least one item")]
    public List<CreateSaleItemDto> SaleItems { get; set; } = new();
}

/// <summary>
/// DTO for creating a sale item
/// </summary>
public class CreateSaleItemDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required(ErrorMessage = "Product ID is required")]
    public int ProductId { get; set; }

    /// <summary>
    /// Quantity of the product
    /// </summary>
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}

/// <summary>
/// DTO for updating sale information
/// </summary>
public class UpdateSaleDto
{
    /// <summary>
    /// Customer name
    /// </summary>
    [StringLength(200, ErrorMessage = "Customer name cannot exceed 200 characters")]
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer email
    /// </summary>
    [StringLength(100, ErrorMessage = "Customer email cannot exceed 100 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Notes or comments about the sale
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }

    /// <summary>
    /// Tax amount applied to the sale
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Tax amount cannot be negative")]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Discount amount applied to the sale
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Discount amount cannot be negative")]
    public decimal DiscountAmount { get; set; }
}

/// <summary>
/// DTO for returning sale information
/// </summary>
public class SaleDto
{
    /// <summary>
    /// Unique identifier for the sale
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique sale number for tracking
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the sale occurred
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Total amount of the sale
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Tax amount applied to the sale
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Discount amount applied to the sale
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Final amount after tax and discount
    /// </summary>
    public decimal FinalAmount { get; set; }

    /// <summary>
    /// Status of the sale
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Customer information
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer email
    /// </summary>
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Notes or comments about the sale
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Sale items
    /// </summary>
    public List<SaleItemDto> SaleItems { get; set; } = new();

    /// <summary>
    /// Payment information
    /// </summary>
    public PaymentInfoDto? PaymentInfo { get; set; }

    /// <summary>
    /// Timestamp when the sale was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the sale was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for sale list items
/// </summary>
public class SaleListItemDto
{
    /// <summary>
    /// Unique identifier for the sale
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique sale number for tracking
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the sale occurred
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Final amount after tax and discount
    /// </summary>
    public decimal FinalAmount { get; set; }

    /// <summary>
    /// Status of the sale
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Customer name
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Number of items in the sale
    /// </summary>
    public int ItemCount { get; set; }
}

/// <summary>
/// DTO for sale items
/// </summary>
public class SaleItemDto
{
    /// <summary>
    /// Unique identifier for the sale item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Quantity of the product sold
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit at the time of sale
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total price for this item
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Product information
    /// </summary>
    public ProductListItemDto Product { get; set; } = null!;
}

/// <summary>
/// DTO for payment information
/// </summary>
public class PaymentInfoDto
{
    /// <summary>
    /// Unique identifier for the payment info
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Payment method used
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Amount paid
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Transaction ID from payment processor
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// Payment reference number
    /// </summary>
    public string? PaymentReference { get; set; }

    /// <summary>
    /// When the payment was processed
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Additional payment details or notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for creating payment information
/// </summary>
public class CreatePaymentInfoDto
{
    /// <summary>
    /// Payment method used
    /// </summary>
    [Required(ErrorMessage = "Payment method is required")]
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Amount paid
    /// </summary>
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment reference number
    /// </summary>
    [StringLength(50, ErrorMessage = "Payment reference cannot exceed 50 characters")]
    public string? PaymentReference { get; set; }

    /// <summary>
    /// Additional payment details or notes
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
