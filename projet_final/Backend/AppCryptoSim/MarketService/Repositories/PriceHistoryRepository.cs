using MarketService.Data;
using MarketService.Models;
using MarketService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketService.Repositories;

public class PriceHistoryRepository : IPriceHistoryRepository
{

    private readonly MarketDbContext _context;

    public PriceHistoryRepository(MarketDbContext context)
    {
        _context = context;
    }

    public async Task AddPriceHistoryAsync(PriceHistory priceHistory)
    {
        _context.PriceHistory.Add(priceHistory);
        await _context.SaveChangesAsync();
    }

    public Task<List<PriceHistory>> GetPriceHistoryAsync(string symbol, int limit = 50, int skip = 0)
    {

        return _context.PriceHistory
            .Where(priceHistory => priceHistory.CryptoSymbol == symbol)
            .OrderByDescending(priceHistory => priceHistory.RecordedAt)
            .Skip(skip)
            .Take(limit)
            .ToListAsync();
    }
    
    public async Task<List<PriceHistory>> GetPriceHistorySnapshotAsync(DateTime date)
    {
        return await _context.PriceHistory
            .Where(ph => ph.RecordedAt <= date)
            .GroupBy(ph => ph.CryptoSymbol)
            .Select(g => g
                .OrderByDescending(ph => ph.RecordedAt)
                .First())
            .ToListAsync();
    }

}
