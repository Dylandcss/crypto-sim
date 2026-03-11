using PortfolioService.Models;

namespace PortfolioService.Repositories;

public interface IPortfolioRepository
{
    // HOLDINGS
    Task<List<Holding>> GetUserHoldingsAsync(int userId);
    Task<Holding?> GetHoldingAsync(int userId, string cryptoSymbol);
    Task AddHoldingAsync(Holding holding);
    Task UpdateHoldingAsync(Holding holding);
    Task DeleteHoldingAsync(Holding holding);

    // TRANSACTIONS
    Task<List<Transaction>> GetUserTransactionsAsync(int userId);
    Task AddTransactionAsync(Transaction transaction);
}