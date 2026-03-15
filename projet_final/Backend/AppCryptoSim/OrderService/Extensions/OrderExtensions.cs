using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Extensions;

public static class OrderExtensions
{

    public static OrderResponse ToDto(this Order order)
    {
        return new OrderResponse (
            OrderId: order.Id,
            CryptoSymbol: order.CryptoSymbol,
            Type: order.Type,
            Quantity: order.Quantity,
            Price: order.Price,
            Total: order.Total,
            Status: order.Status,
            ExecutedAt: order.ExecutedAt ?? order.CreatedAt,
            LimitPrice: order.LimitPrice
        );
    }

}
