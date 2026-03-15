using CryptoSim.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Repositories.Interfaces;


namespace OrderService.Repositories;


public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order> AddOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders.FindAsync(orderId);
    }

    public async Task<List<Order>> GetOrdersByUserId(int userId)
    {
        return await _context.Orders.Where(o => o.UserId == userId).OrderBy(o => o.CreatedAt).ToListAsync();
    }

    public async Task<List<Order>> GetPendingLimitOrdersAsync()
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Pending && o.LimitPrice.HasValue)
            .ToListAsync();
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, DateTime? executedAt = null)
    {
        await _context.Orders
            .Where(o => o.Id == orderId)
            .ExecuteUpdateAsync(prop => prop
                .SetProperty(o => o.Status, newStatus)
                .SetProperty(o => o.ExecutedAt, executedAt ?? DateTime.UtcNow)
            );
    }
}
