using MarketService.Dtos;
using MarketService.Models;

namespace MarketService.Services.Interfaces;

public interface ICryptosService
{
    public Task<CryptoResponse?> GetCryptoByIdAsync(int id);

    public Task<CryptoResponse?> GetCryptoBySymbolAsync(string symbol);

    public Task<List<CryptoResponse>> GetAllCryptosAsync();

    public Task<CryptoResponse> UpdatePriceAsync(int id, PriceUpdate crypto);
}
