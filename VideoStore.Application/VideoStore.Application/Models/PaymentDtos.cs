namespace VideoStore.Application.Models;

// Request to confirm payment
public class PaymentConfirmationRequest
{
    public string TransactionId { get; set; } = string.Empty;
}

// Response from Banking API when verifying a transaction
public class BankingTransactionResponse
{
    public string Id { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public decimal Balance { get; set; }
}
