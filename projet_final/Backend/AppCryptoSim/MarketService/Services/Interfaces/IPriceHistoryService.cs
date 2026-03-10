using MarketService.Dtos;
using MarketService.Models;

namespace MarketService.Services.Interfaces;

public interface IPriceHistoryService
{
    
    Task<List<PriceHistoryResponse>> GetPriceHistoryListAsync(string symbol, int limit = 50, int skip = 0);
    
    Task AddNewPriceHistoryAsync(string symbol, decimal price);

    Task<List<PriceHistory>> GetPriceHistorySnapshotAsync(DateTime date);
}
