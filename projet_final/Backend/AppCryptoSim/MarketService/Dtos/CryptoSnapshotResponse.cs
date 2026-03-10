namespace MarketService.Dtos;

public record CryptoSnapshotResponse
(
    int Id,
    string Symbol,
    string Name,
    decimal CurrentPrice,
    DateTime LastUpdated
);




