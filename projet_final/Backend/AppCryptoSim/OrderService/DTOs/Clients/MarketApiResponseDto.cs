namespace OrderService.Dtos;

public record MarketApiResponseDto
(
    string Symbol,
    decimal CurrentPrice
);
