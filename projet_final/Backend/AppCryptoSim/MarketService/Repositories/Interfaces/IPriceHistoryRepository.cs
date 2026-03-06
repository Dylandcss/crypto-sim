using MarketService.Models;

namespace MarketService.Repositories.Interfaces;

public interface IPriceHistoryRepository
{

    Task AddPriceHistoryAsync(PriceHistory priceHistory);

    Task<List<PriceHistory>> GetPriceHistoryAsync(string symbol, int limit = 50, int skip = 0);
}
