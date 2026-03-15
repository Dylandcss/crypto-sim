using CryptoSim.Shared.Enums;

namespace OrderService.Dtos.Clients;

public record InternalUpdatePortfolioHoldingRequestDto(
    int UserId,
    string CryptoSymbol,
    decimal Quantity,
    decimal PriceAtTime,
    OrderType Type
);
