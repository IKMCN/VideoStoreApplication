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
            var response = await _httpClient.GetAsync($"/api/transactions/{transactionId}");

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
}
