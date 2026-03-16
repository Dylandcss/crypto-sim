using CryptoSim.Shared.Enums;

namespace PortfolioService.Dtos;

public record UpdatePortfolioDto(
    string CryptoSymbol,
    decimal Quantity,
    decimal PriceAtTime,
    OrderType Type
    );