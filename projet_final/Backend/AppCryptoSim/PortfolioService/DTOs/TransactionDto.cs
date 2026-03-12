using CryptoSim.Shared.Enums;

namespace PortfolioService.Dtos;

public record TransactionDto(
    int Id,
    string CryptoSymbol,
    OrderType Type,
    decimal Quantity,
    decimal PriceAtTime,
    decimal Total,
    DateTime ExecutedAt
    );