namespace OrderService.Dtos.Clients;

public record PortfolioApiHoldingRequestDto
(
    string CryptoSymbol,
    decimal Quantity,
    decimal PurchasePrice
);
