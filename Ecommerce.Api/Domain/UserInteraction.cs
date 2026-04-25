using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Domain;

public class UserInteraction
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    [Required]
    public string Action { get; set; } = string.Empty;

    public int? ProductId { get; set; }

    public string? Metadata { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
