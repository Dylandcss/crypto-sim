namespace MarketService.Dtos;

public record PriceHistoryResponse(
    string CryptoSymbol,
    decimal Price,
    DateTime RecordedAt
);
