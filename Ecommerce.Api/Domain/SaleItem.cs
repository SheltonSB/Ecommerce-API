using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Api.Domain;

/// <summary>
/// Represents an item within a sale transaction
/// </summary>
public class SaleItem : Entity
{
    /// <summary>
    /// Foreign key for the sale
    /// </summary>
    [Required]
    public int SaleId { get; set; }

    /// <summary>
    /// Foreign key for the product
    /// </summary>
    [Required]
    public int ProductId { get; set; }

    /// <summary>
    /// Quantity of the product sold
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit at the time of sale (immutable for historical accuracy)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total price for this item (Quantity * UnitPrice)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Navigation property for the sale
    /// </summary>
    [ForeignKey("SaleId")]
    public virtual Sale Sale { get; set; } = null!;

    /// <summary>
    /// Navigation property for the product
    /// </summary>
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// Calculates the total price based on quantity and unit price
    /// </summary>
    public void CalculateTotalPrice()
    {
        TotalPrice = Quantity * UnitPrice;
    }

    /// <summary>
    /// Updates the quantity and recalculates total price
    /// </summary>
    /// <param name="newQuantity">The new quantity</param>
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        Quantity = newQuantity;
        CalculateTotalPrice();
        UpdateTimestamp();
    }

    /// <summary>
    /// Validates the sale item data
    /// </summary>
    /// <returns>True if the sale item is valid</returns>
    public bool IsValid()
    {
        return Quantity > 0 && UnitPrice > 0 && TotalPrice > 0;
    }
}
