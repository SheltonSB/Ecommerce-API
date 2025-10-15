using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Domain;

/// <summary>
/// Represents a product category in the e-commerce system
/// </summary>
public class Category : Entity
{
    /// <summary>
    /// The name of the category
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the category
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Navigation property for products in this category
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    /// <summary>
    /// Validates the category data
    /// </summary>
    /// <returns>True if the category is valid</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && Name.Length >= 2 && Name.Length <= 100;
    }
}
