using CryptoSim.Shared.Enums;

namespace OrderService.Dtos.Clients;


public record UpdatePortfolioHoldingRequestDto (
    string CryptoSymbol,
    decimal Quantity,      
    decimal PriceAtTime,
    OrderType type
);
