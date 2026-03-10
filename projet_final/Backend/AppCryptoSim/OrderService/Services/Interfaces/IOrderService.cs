using OrderService.Dtos;

namespace OrderService.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> AddOrderAsync(int userId, OrderRequest request, string token);

    Task<List<OrderResponse>> GetUserOrdersAsync(int userId);

    Task<OrderResponse> GetOrderByIdAsync(int orderId, int userId);

    Task<bool> DeleteOrderAsync(int orderId);
}
