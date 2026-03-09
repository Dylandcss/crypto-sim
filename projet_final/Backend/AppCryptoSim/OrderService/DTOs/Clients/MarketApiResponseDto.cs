namespace OrderService.Dtos.Clients;

public record MarketApiResponseDto
(
    string Symbol,
    decimal CurrentPrice
);
