using CryptoSim.Shared.Enums;

namespace PortfolioService.Dtos;

public record InternalUpdatePortfolioDto(
    int UserId,
    string CryptoSymbol,
    decimal Quantity,
    decimal PriceAtTime,
    OrderType Type
);
