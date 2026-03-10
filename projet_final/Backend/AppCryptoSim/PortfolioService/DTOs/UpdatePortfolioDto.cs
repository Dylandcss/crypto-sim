using CryptoSim.Shared.Enums;

namespace PortfolioService.Dtos;

public record UpdatePortfolioDto(
    string CryptoSymbol,
    OrderType Type,
    decimal Quantity,
    decimal Price
    );