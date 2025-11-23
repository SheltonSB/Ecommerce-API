using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Api.Domain;

/// <summary>
/// Represents a sale transaction in the e-commerce system
/// </summary>
public class Sale : Entity
{
    /// <summary>
    /// Unique sale number for tracking
    /// </summary>
    [Required]
    [StringLength(50)]
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the sale occurred
    /// </summary>
    [Required]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Total amount of the sale (calculated from items)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Tax amount applied to the sale
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } = 0;

    /// <summary>
    /// Discount amount applied to the sale
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0;

    /// <summary>
    /// Final amount after tax and discount
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal FinalAmount { get; set; }

    /// <summary>
    /// Status of the sale
    /// </summary>
    [Required]
    public SaleStatus Status { get; set; } = SaleStatus.Pending;

    /// <summary>
    /// Customer information
    /// </summary>
    [StringLength(200)]
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer email
    /// </summary>
    [StringLength(100)]
    [EmailAddress]
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Customer city for geospatial analysis
    /// </summary>
    [StringLength(100)]
    public string? CustomerCity { get; set; }

    /// <summary>
    /// Customer state or province for geospatial analysis
    /// </summary>
    [StringLength(100)]
    public string? CustomerState { get; set; }

    /// <summary>
    /// Customer country for geospatial analysis
    /// </summary>
    [StringLength(100)]
    public string? CustomerCountry { get; set; }

    /// <summary>
    /// Notes or comments about the sale
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property for sale items
    /// </summary>
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    /// <summary>
    /// Navigation property for payment information
    /// </summary>
    public virtual PaymentInfo? PaymentInfo { get; set; }

    /// <summary>
    /// Calculates the total amount from sale items
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = SaleItems.Sum(item => item.TotalPrice);
        CalculateFinalAmount();
    }

    /// <summary>
    /// Calculates the final amount after tax and discount
    /// </summary>
    public void CalculateFinalAmount()
    {
        FinalAmount = TotalAmount + TaxAmount - DiscountAmount;
        if (FinalAmount < 0) FinalAmount = 0;
    }

    /// <summary>
    /// Adds an item to the sale
    /// </summary>
    /// <param name="product">The product to add</param>
    /// <param name="quantity">Quantity to add</param>
    public void AddSaleItem(Product product, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot modify completed or cancelled sale");

        var existingItem = SaleItems.FirstOrDefault(item => item.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            existingItem.CalculateTotalPrice();
        }
        else
        {
            var saleItem = new SaleItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = quantity,
                UnitPrice = product.Price,
                SaleId = Id
            };
            saleItem.CalculateTotalPrice();
            SaleItems.Add(saleItem);
        }

        CalculateTotalAmount();
        UpdateTimestamp();
    }

    /// <summary>
    /// Completes the sale and reduces product stock
    /// </summary>
    public void CompleteSale()
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Only pending sales can be completed");

        foreach (var item in SaleItems)
        {
            item.Product.ReduceStock(item.Quantity);
        }

        Status = SaleStatus.Completed;
        UpdateTimestamp();
    }

    /// <summary>
    /// Cancels the sale
    /// </summary>
    public void CancelSale()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed sale");

        Status = SaleStatus.Cancelled;
        UpdateTimestamp();
    }

    /// <summary>
    /// Validates the sale data
    /// </summary>
    /// <returns>True if the sale is valid</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(SaleNumber) &&
               SaleItems.Any() &&
               TotalAmount >= 0 &&
               FinalAmount >= 0;
    }
}

/// <summary>
/// Enum representing the status of a sale
/// </summary>
public enum SaleStatus
{
    /// <summary>
    /// Sale is pending completion
    /// </summary>
    Pending,

    /// <summary>
    /// Sale has been completed
    /// </summary>
    Completed,

    /// <summary>
    /// Sale has been cancelled
    /// </summary>
    Cancelled,

    /// <summary>
    /// Sale has been refunded
    /// </summary>
    Refunded
}
