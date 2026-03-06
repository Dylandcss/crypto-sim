namespace MarketService.Dtos;

public record CryptoResponse(
    int Id,
    string Symbol,
    string Name,
    decimal CurrentPrice,
    DateTime LastUpdated
);
