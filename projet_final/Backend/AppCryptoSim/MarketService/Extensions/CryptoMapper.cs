using MarketService.Dtos;
using MarketService.Models;

namespace MarketService.Extensions;

public static class CryptoMapper
{

    public static Crypto ToModel(this PriceUpdate dto)
    {
        return new Crypto()
        {
            Symbol = dto.Symbol,
            Name = dto.Name,
            CurrentPrice = dto.Price,
            LastUpdated = dto.UpdatedAt
        };
    }


    public static CryptoResponse ToDto(this Crypto crypto)
    {
        return new CryptoResponse(
            Id: crypto.Id,
            Symbol: crypto.Symbol,
            Name: crypto.Name,
            CurrentPrice: crypto.CurrentPrice,
            LastUpdated: crypto.LastUpdated
        );
    }

    public static PriceUpdate ToDto(this CryptoResponse crypto, decimal price)
    {
        return new PriceUpdate(
            Symbol: crypto.Symbol,
            Name: crypto.Name,
            Price: price,
            UpdatedAt: DateTime.UtcNow
        );
    }


    public static PriceHistoryResponse ToDto(this PriceHistory history)
    {
        return new PriceHistoryResponse(
            CryptoSymbol: history.CryptoSymbol,
            Price: history.Price,
            RecordedAt: history.RecordedAt
        );
    }


}
