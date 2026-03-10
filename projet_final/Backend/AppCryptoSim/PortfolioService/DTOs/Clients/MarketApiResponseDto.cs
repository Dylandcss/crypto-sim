namespace PortfolioService.Dtos.Clients;

public record MarketApiResponseDto
(
    string Symbol,
    decimal CurrentPrice
);