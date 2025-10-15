using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Api.Domain;

/// <summary>
/// Represents the price history for a product
/// </summary>
public class PriceHistory : Entity
{
    /// <summary>
    /// Foreign key for the product
    /// </summary>
    [Required]
    public int ProductId { get; set; }

    /// <summary>
    /// The previous price
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal OldPrice { get; set; }

    /// <summary>
    /// The new price
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal NewPrice { get; set; }

    /// <summary>
    /// When the price change occurred
    /// </summary>
    [Required]
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property for the product
    /// </summary>
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// Calculates the percentage change in price
    /// </summary>
    /// <returns>The percentage change</returns>
    public decimal GetPercentageChange()
    {
        if (OldPrice == 0) return 0;
        return ((NewPrice - OldPrice) / OldPrice) * 100;
    }

    /// <summary>
    /// Determines if the price increased
    /// </summary>
    /// <returns>True if price increased</returns>
    public bool IsPriceIncrease()
    {
        return NewPrice > OldPrice;
    }

    /// <summary>
    /// Determines if the price decreased
    /// </summary>
    /// <returns>True if price decreased</returns>
    public bool IsPriceDecrease()
    {
        return NewPrice < OldPrice;
    }
}
