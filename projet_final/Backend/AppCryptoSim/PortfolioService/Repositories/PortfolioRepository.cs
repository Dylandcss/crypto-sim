using Microsoft.EntityFrameworkCore;
using PortfolioService.Data;
using PortfolioService.Models;

namespace PortfolioService.Repositories;
 
public class PortfolioRepository : IPortfolioRepository
{
    private readonly PortfolioDbContext _context;

    public PortfolioRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<List<Holding>> GetUserHoldingsAsync(int userId)
    {
        return await _context.Holdings
            .Where(h => h.UserId == userId)
            .ToListAsync();
    }

    public async Task<Holding?> GetHoldingAsync(int userId, string cryptoSymbol)
    {
        return await _context.Holdings
            .FirstOrDefaultAsync(h => h.UserId == userId && h.CryptoSymbol == cryptoSymbol);
    }

    public async Task AddHoldingAsync(Holding holding)
    {
        await _context.Holdings.AddAsync(holding);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateHoldingAsync(Holding holding)
    {
        _context.Holdings.Update(holding);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Transaction>> GetUserTransactionsAsync(int userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.ExecutedAt)
            .ToListAsync();
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }
    
}