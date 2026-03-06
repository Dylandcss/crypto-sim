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

    public async Task<List<Crypto>> GetAllCryptosAsync()
    {
        return await _context.Cryptos.ToListAsync();
    }

    public Task<Crypto?> GetCryptoByIdAsync(int id)
    {
        return _context.Cryptos.FirstOrDefaultAsync(crypto => crypto.Id == id);
    }

    public Task<Crypto?> GetCryptoBySymbolAsync(string symbol)
    {
        return _context.Cryptos.FirstOrDefaultAsync(crypto => crypto.Symbol == symbol);
    }

    public async Task UpdatePriceAsync(Crypto crypto)
    {
        _context.Cryptos.Update(crypto);
        await _context.SaveChangesAsync();
    }
}
