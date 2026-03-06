using OrderService.Models;

namespace OrderService.DTOs;

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