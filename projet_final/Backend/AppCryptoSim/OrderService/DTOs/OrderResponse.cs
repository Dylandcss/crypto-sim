using CryptoSim.Shared.Enums;


namespace OrderService.Dtos;


public record OrderResponse(
    int OrderId,
    string CryptoSymbol,
    OrderType Type,
    decimal Quantity,
    decimal Price,
    decimal Total,
    OrderStatus Status,
    DateTime ExecutedAt
);