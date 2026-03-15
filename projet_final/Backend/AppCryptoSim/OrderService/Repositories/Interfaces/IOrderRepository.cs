using CryptoSim.Shared.Enums;
using OrderService.Models;

namespace OrderService.Repositories.Interfaces;

public interface IOrderRepository
{

    Task<Order> AddOrderAsync(Order order);

    Task<List<Order>> GetOrdersByUserId(int userId);


    Task<Order?> GetOrderByIdAsync(int orderId);

    Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, DateTime? executedAt = null);

    Task DeleteOrderAsync(int orderId);

    Task<List<Order>> GetPendingLimitOrdersAsync();

}
