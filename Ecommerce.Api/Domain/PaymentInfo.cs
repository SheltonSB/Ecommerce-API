using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Api.Domain;

/// <summary>
/// Represents payment information for a sale
/// </summary>
public class PaymentInfo : Entity
{
    /// <summary>
    /// Foreign key for the sale
    /// </summary>
    [Required]
    public int SaleId { get; set; }

    /// <summary>
    /// Payment method used
    /// </summary>
    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Amount paid
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>
    /// Transaction ID from payment processor
    /// </summary>
    [StringLength(100)]
    public string? TransactionId { get; set; }

    /// <summary>
    /// Payment reference number
    /// </summary>
    [StringLength(50)]
    public string? PaymentReference { get; set; }

    /// <summary>
    /// When the payment was processed
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Additional payment details or notes
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property for the sale
    /// </summary>
    public virtual Sale Sale { get; set; } = null!;

    /// <summary>
    /// Marks the payment as processed
    /// </summary>
    /// <param name="transactionId">Transaction ID from payment processor</param>
    public void MarkAsProcessed(string? transactionId = null)
    {
        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        ProcessedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    /// <summary>
    /// Marks the payment as failed
    /// </summary>
    /// <param name="reason">Reason for failure</param>
    public void MarkAsFailed(string? reason = null)
    {
        Status = PaymentStatus.Failed;
        Notes = reason;
        UpdateTimestamp();
    }

    /// <summary>
    /// Marks the payment as refunded
    /// </summary>
    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
        UpdateTimestamp();
    }

    /// <summary>
    /// Validates the payment information
    /// </summary>
    /// <returns>True if the payment info is valid</returns>
    public bool IsValid()
    {
        return Amount > 0;
    }
}

/// <summary>
/// Enum representing payment methods
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Credit card payment
    /// </summary>
    CreditCard,

    /// <summary>
    /// Debit card payment
    /// </summary>
    DebitCard,

    /// <summary>
    /// PayPal payment
    /// </summary>
    PayPal,

    /// <summary>
    /// Bank transfer
    /// </summary>
    BankTransfer,

    /// <summary>
    /// Cash payment
    /// </summary>
    Cash,

    /// <summary>
    /// Digital wallet payment
    /// </summary>
    DigitalWallet,

    /// <summary>
    /// Cryptocurrency payment
    /// </summary>
    Cryptocurrency
}

/// <summary>
/// Enum representing payment status
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment is pending
    /// </summary>
    Pending,

    /// <summary>
    /// Payment has been completed successfully
    /// </summary>
    Completed,

    /// <summary>
    /// Payment has failed
    /// </summary>
    Failed,

    /// <summary>
    /// Payment has been refunded
    /// </summary>
    Refunded,

    /// <summary>
    /// Payment has been cancelled
    /// </summary>
    Cancelled
}
