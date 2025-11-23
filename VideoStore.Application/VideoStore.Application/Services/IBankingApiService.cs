using VideoStore.Application.Models;

namespace VideoStore.Application.Services;

public interface IBankingApiService
{
    Task<BankingTransactionResponse?> VerifyTransactionAsync(string transactionId);
    Task<BankingTransactionResponse?> FindMatchingTransactionAsync(string accountNumber, decimal amount, DateTime since);
}