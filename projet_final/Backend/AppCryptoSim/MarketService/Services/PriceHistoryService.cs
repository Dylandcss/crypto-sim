using CryptoSim.Shared.Exceptions;
using MarketService.Dtos;
using MarketService.Extensions;
using MarketService.Models;
using MarketService.Repositories.Interfaces;
using MarketService.Services.Interfaces;


namespace MarketService.Services;


public class PriceHistoryService : IPriceHistoryService
{
    private readonly IPriceHistoryRepository _historyRepository;
    private readonly ICryptosRepository _cryptosRepository;

    public PriceHistoryService(IPriceHistoryRepository historyRepository, ICryptosRepository cryptosRepository) 
    {
        _historyRepository = historyRepository;
        _cryptosRepository = cryptosRepository;
    }

    public async Task<List<PriceHistoryResponse>> GetPriceHistoryListAsync(string symbol, int limit = 50)
    {
        var cryptoExists = await _cryptosRepository.IsCryptoExistsAsync(symbol);

        if (!cryptoExists) throw new NotFoundException($"La cryptomonnaie '{symbol}' est introuvable.");

        var historyList = await _historyRepository.GetPriceHistoryAsync(symbol, limit);

        return historyList.Select(priceHistory => priceHistory.ToDto()).ToList();
    }

    public async Task AddNewPriceHistoryAsync(string symbol, decimal price)
    {
        var history = new PriceHistory
        {
            CryptoSymbol = symbol,
            Price = price,
            RecordedAt = DateTime.UtcNow
        };

        await _historyRepository.AddPriceHistoryAsync(history);
    }

}
