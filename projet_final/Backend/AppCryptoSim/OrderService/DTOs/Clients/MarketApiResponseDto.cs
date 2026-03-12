namespace OrderService.Dtos.Clients;

public record MarketApiResponseDto
(
    string Symbol,
    string Name,
    decimal CurrentPrice
);
