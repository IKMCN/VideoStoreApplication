using System.Text.Json;
using Microsoft.Extensions.Logging;
using VideoStore.Application.Models;

namespace VideoStore.Application.Services;

public class BankingApiService : IBankingApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BankingApiService> _logger;

    public BankingApiService(HttpClient httpClient, ILogger<BankingApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BankingTransactionResponse?> VerifyTransactionAsync(string transactionId)
    {
        try
        {
            _logger.LogInformation("Verifying transaction {TransactionId} with Banking API", transactionId);

            // Call Banking API to verify transaction exists
            var response = await _httpClient.GetAsync($"/api/transactions/{transactionId}/verify");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Banking API returned {StatusCode} for transaction {TransactionId}",
                    response.StatusCode, transactionId);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var transaction = JsonSerializer.Deserialize<BankingTransactionResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Successfully verified transaction {TransactionId}", transactionId);
            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying transaction {TransactionId} with Banking API", transactionId);
            return null;
        }
    }

    public async Task<BankingTransactionResponse?> FindMatchingTransactionAsync(
        string accountNumber,
        decimal amount,
        DateTime since)
    {
        try
        {
            _logger.LogInformation(
                "Searching for matching transaction: Account={AccountNumber}, Amount={Amount}, Since={Since}",
                accountNumber, amount, since);

            // Get all transactions for the account
            // Note: This assumes there's an endpoint to get transactions by account
            // You might need to adjust based on your Banking API structure
            var response = await _httpClient.GetAsync($"/api/accounts/{accountNumber}/transactions");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get transactions for account {AccountNumber}", accountNumber);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var transactionsWrapper = JsonSerializer.Deserialize<TransactionsListResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (transactionsWrapper?.Items == null)
            {
                return null;
            }

            // Find matching transaction
            // Looking for: withdrawal/debit of exact amount, created after rental
            var match = transactionsWrapper.Items
                .Where(t => t.Timestamp >= since)
                .Where(t => Math.Abs(t.Amount - amount) < 0.01m)
                .Where(t => t.Type.Contains("Withdrawal", StringComparison.OrdinalIgnoreCase) ||
                           t.Type.Contains("Atm", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.Timestamp)
                .FirstOrDefault();

            if (match != null)
            {
                _logger.LogInformation("Found matching transaction {TransactionId}", match.Id);
            }

            return match;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding matching transaction");
            return null;
        }
    }
}

// Helper class for deserializing list of transactions
public class TransactionsListResponse
{
    public List<BankingTransactionResponse> Items { get; set; } = new();
}