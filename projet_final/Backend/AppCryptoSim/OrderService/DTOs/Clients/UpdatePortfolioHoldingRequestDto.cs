namespace OrderService.Dtos.Clients;


public record UpdatePortfolioHoldingRequestDto(
    string CryptoSymbol,
    decimal Quantity,      
    decimal PriceAtTime 
);
