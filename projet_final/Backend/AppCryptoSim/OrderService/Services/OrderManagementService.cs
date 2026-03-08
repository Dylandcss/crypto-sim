using CryptoSim.Shared.Enums;
using CryptoSim.Shared.Exceptions;
using OrderService.Dtos;
using OrderService.Extensions;
using OrderService.Models;
using OrderService.Repositories.Interfaces;
using OrderService.Services.Clients;
using OrderService.Services.Interfaces;

namespace OrderService.Services;

public class OrderManagementService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly AuthApiClient _authClient;
    private readonly MarketApiClient _marketClient;
    private readonly PortfolioApiClient _portfolioClient;

    public OrderManagementService(
        IOrderRepository repository,
        AuthApiClient authClient,
        MarketApiClient marketClient,
        PortfolioApiClient portfolioClient)
    {
        _repository = repository;
        _authClient = authClient;
        _marketClient = marketClient;
        _portfolioClient = portfolioClient;
    }

    public async Task<OrderResponse> AddOrderAsync(int userId, OrderRequest request, string token)
    {
        // Prix actuel
        decimal price = await _marketClient.GetCryptoPriceAsync(token, request.CryptoSymbol);
        if (price <= 0) throw new BadRequestException("Prix indisponible.");

        decimal total = request.Quantity * price;

        // Creer un ordre 
        var order = new Order
        {
            UserId = userId,
            CryptoSymbol = request.CryptoSymbol,
            Type = request.Type,
            Quantity = request.Quantity,
            Price = price,
            Total = total,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        var createdOrder = await _repository.AddOrderAsync(order);
        
        try
        {
            if (request.Type == OrderType.Buy)
            {
                // Achat : Vérifier budget -> Debiter -> Ajouter actif
                var balance = await _authClient.GetUserBalance(token);
                if (balance < total) throw new BadRequestException("Solde insuffisant.");

                await _authClient.UpdateUserBalance(-total, token);

                try
                {
                    await _portfolioClient.UpdateHoldingAsync(token, request.CryptoSymbol, request.Quantity, price);
                }
                catch
                {
                    // Rendre l'argent si le portfolio échoue
                    await _authClient.UpdateUserBalance(total, token);
                    throw;
                }
            }
            else
            {
                // Vente : Verifier stock -> Retirer actif -> Crediter 
                var stock = await _portfolioClient.GetHoldingQuantityAsync(token, request.CryptoSymbol);
                if (stock < request.Quantity) throw new BadRequestException("Quantité insuffisante.");

                await _portfolioClient.UpdateHoldingAsync(token, request.CryptoSymbol, -request.Quantity, price);
                await _authClient.UpdateUserBalance(total, token);
            }

            await _repository.UpdateOrderStatusAsync(createdOrder.Id, OrderStatus.Executed);
            createdOrder.Status = OrderStatus.Executed;
        }
        catch
        {
            await _repository.UpdateOrderStatusAsync(createdOrder.Id, OrderStatus.Rejected);
            createdOrder.Status = OrderStatus.Rejected;
            throw;
        }

        return createdOrder.ToDto();
    }

    public async Task<List<OrderResponse>> GetUserOrdersAsync(int userId) =>
        (await _repository.GetOrdersByUserId(userId)).Select(o => o.ToDto()).ToList();

    public async Task<OrderResponse> GetOrderByIdAsync(int orderId, int userId)
    {
        var order = await _repository.GetOrderByIdAsync(orderId);
        if (order == null || order.UserId != userId) throw new NotFoundException("Commande introuvable.");
        return order.ToDto();
    }
}
