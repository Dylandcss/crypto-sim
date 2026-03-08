using MarketService.Dtos;

namespace MarketService.Services.Interfaces;

public interface IPriceHistoryService
{
    
    Task<List<PriceHistoryResponse>> GetPriceHistoryListAsync(string symbol, int limit = 50);
    
    Task AddNewPriceHistoryAsync(string symbol, decimal price);
}
