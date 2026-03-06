namespace MarketService.Dtos;

public record PriceUpdate(
    string Symbol,
    string Name,
    decimal Price,
    DateTime UpdatedAt
);
