using MarketService.Data;
using MarketService.Models;
using MarketService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketService.Repositories;

public class CryptosRepository : ICryptosRepository
{
    private readonly MarketDbContext _context;

    public CryptosRepository(MarketDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsCryptoExistsAsync(string symbol)
    {
        return await _context.Cryptos.AnyAsync(x => x.Symbol == symbol);
    }

    public async Task<List<Crypto>> GetAllCryptosAsync()
    {
        return await _context.Cryptos.ToListAsync();
    }

    public async Task<Crypto?> GetCryptoByIdAsync(int id)
    {
        return await _context.Cryptos.FindAsync(id);
    }

    public async Task<Crypto?> GetCryptoBySymbolAsync(string symbol)
    {
        return await _context.Cryptos.FirstOrDefaultAsync(crypto => crypto.Symbol == symbol);
    }

    public async Task UpdatePriceAsync(int cryptoId, decimal newPrice)
    {
        await _context.Cryptos
        .Where(c => c.Id == cryptoId)
        .ExecuteUpdateAsync(s => s
            .SetProperty(c => c.CurrentPrice, newPrice)
            .SetProperty(c => c.LastUpdated, DateTime.UtcNow)
        );
    }
}
