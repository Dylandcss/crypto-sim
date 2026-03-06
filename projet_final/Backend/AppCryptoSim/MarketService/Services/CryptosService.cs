using MarketService.Dtos;
using MarketService.Extensions;
using MarketService.Repositories.Interfaces;
using MarketService.Services.Interfaces;

namespace MarketService.Services;

public class CryptosService : ICryptosService
{
    private readonly ICryptosRepository _cryptosRepository;

    public CryptosService(ICryptosRepository cryptosRepository)
    {
        _cryptosRepository = cryptosRepository;
    }

    public async Task<List<CryptoResponse>> GetAllCryptosAsync()
    {
        var result = await _cryptosRepository.GetAllCryptosAsync();
        return result.Select(c => c.ToDto()).ToList();
    }

    public async Task<CryptoResponse?> GetCryptoByIdAsync(int id)
    {
        var result =  await _cryptosRepository.GetCryptoByIdAsync(id);
        return result?.ToDto();
    }

    public async Task<CryptoResponse?> GetCryptoBySymbolAsync(string symbol)
    {
        var result = await _cryptosRepository.GetCryptoBySymbolAsync(symbol);
        return result?.ToDto();
    }

    public async Task<CryptoResponse> UpdatePriceAsync(int id, PriceUpdate priceUpdate)
    {

        var crypto = await _cryptosRepository.GetCryptoByIdAsync(id);

        if (crypto is null) throw new ArgumentException("Update prix crypto impossible: la crypto avec l'id {id} est introuvable");


        crypto.Symbol = priceUpdate.Symbol;
        crypto.LastUpdated = priceUpdate.UpdatedAt;
        crypto.CurrentPrice = priceUpdate.Price;
        crypto.Name = priceUpdate.Name;

        
        
        await (_cryptosRepository.UpdatePriceAsync(crypto));
        return crypto.ToDto();
    }
}
