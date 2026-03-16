namespace OrderService.Dtos.Clients;

public record PortfolioApiHoldingResponseDto
(
    string CryptoSymbol,
    decimal Quantity
);
