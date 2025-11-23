namespace VideoStore.Application.Models;

public class Rental
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public Guid VideoId { get; set; }
    public DateTime RentedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }  // Nullable - null means not returned
    public decimal LateFee { get; set; }
    
    // Payment tracking fields
    public string? PaymentTransactionId { get; set; }  // Transaction ID from Banking API
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public decimal RentalAmount { get; set; }  // Amount to be paid for the rental
}

public enum PaymentStatus
{
    Pending,    // Rental created, waiting for payment
    Paid,       // Payment confirmed
    Failed,     // Payment attempt failed
    Refunded    // Payment was refunded
}
