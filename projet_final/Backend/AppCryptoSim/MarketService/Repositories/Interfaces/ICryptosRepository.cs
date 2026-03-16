using MarketService.Models;


namespace MarketService.Repositories.Interfaces;


public interface ICryptosRepository
{

    public Task<Crypto?> GetCryptoByIdAsync(int id);

    public Task<Crypto?> GetCryptoBySymbolAsync(string symbol);

    public Task<List<Crypto>> GetAllCryptosAsync();
    
    public Task UpdatePriceAsync(int cryptoId, decimal newPrice);

    Task<bool> IsCryptoExistsAsync(string symbol);

}
