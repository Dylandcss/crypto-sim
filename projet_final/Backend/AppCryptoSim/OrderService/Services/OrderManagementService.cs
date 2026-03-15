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
        // Ordre limite : enregistrer comme Pending sans exécuter
        if (request.LimitPrice.HasValue)
        {
            var limitOrder = new Order
            {
                UserId = userId,
                CryptoSymbol = request.CryptoSymbol,
                Type = request.Type,
                Quantity = request.Quantity,
                Price = request.LimitPrice.Value,
                Total = request.Quantity * request.LimitPrice.Value,
                LimitPrice = request.LimitPrice.Value,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            return (await _repository.AddOrderAsync(limitOrder)).ToDto();
        }

        // Ordre au marché : exécuter immédiatement
        decimal price = await _marketClient.GetCryptoPriceAsync(request.CryptoSymbol, token);
        if (price <= 0) throw new BadRequestException("Prix indisponible.");

        var order = new Order
        {
            UserId = userId,
            CryptoSymbol = request.CryptoSymbol,
            Type = request.Type,
            Quantity = request.Quantity,
            Price = price,
            Total = request.Quantity * price,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        var createdOrder = await _repository.AddOrderAsync(order);

        try
        {
            await ExecuteOrderCoreAsync(createdOrder, price, token);
            await _repository.UpdateOrderStatusAsync(createdOrder.Id, OrderStatus.Executed, DateTime.UtcNow);
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

    public async Task<bool> DeleteOrderAsync(int orderId, int userId)
    {
        var order = await _repository.GetOrderByIdAsync(orderId);
        if (order is null) throw new NotFoundException($"Commande {orderId} introuvable.");

        if (order.UserId != userId)
            throw new ForbiddenException("Vous n'êtes pas autorisé à annuler cette commande.");

        if (order.Status != OrderStatus.Pending)
            throw new BadRequestException($"La commande {orderId} ne peut pas être annulée : elle n'est plus en attente.");

        await _repository.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled, null);
        return true;
    }

    public async Task ExecuteLimitOrdersAsync()
    {
        var pendingOrders = await _repository.GetPendingLimitOrdersAsync();

        foreach (var order in pendingOrders)
        {
            try
            {
                decimal currentPrice = await _marketClient.GetCryptoPriceAsync(order.CryptoSymbol, "");
                if (currentPrice <= 0) continue;

                bool shouldExecute = order.Type == OrderType.Buy
                    ? currentPrice <= order.LimitPrice!.Value
                    : currentPrice >= order.LimitPrice!.Value;

                if (!shouldExecute) continue;

                order.Price = currentPrice;
                order.Total = order.Quantity * currentPrice;

                await ExecuteOrderCoreInternalAsync(order, currentPrice);
                await _repository.UpdateOrderStatusAsync(order.Id, OrderStatus.Executed, DateTime.UtcNow);
            }
            catch
            {
                // Conserver l'ordre en Pending pour réessayer au prochain cycle
            }
        }
    }

    // Exécution via JWT utilisateur (ordres au marché)
    private async Task ExecuteOrderCoreAsync(Order order, decimal price, string token)
    {
        decimal total = order.Quantity * price;

        if (order.Type == OrderType.Buy)
        {
            var balance = await _authClient.GetUserBalance(token);
            if (balance < total) throw new BadRequestException("Solde insuffisant.");

            await _authClient.UpdateUserBalance(-total, token);
            try
            {
                await _portfolioClient.UpdateHoldingAsync(token, order.CryptoSymbol, order.Quantity, price, OrderType.Buy);
            }
            catch
            {
                await _authClient.UpdateUserBalance(total, token);
                throw;
            }
        }
        else
        {
            var stock = await _portfolioClient.GetHoldingQuantityAsync(token, order.CryptoSymbol);
            if (stock < order.Quantity) throw new BadRequestException("Quantité insuffisante.");

            await _portfolioClient.UpdateHoldingAsync(token, order.CryptoSymbol, order.Quantity, price, OrderType.Sell);
            await _authClient.UpdateUserBalance(total, token);
        }
    }

    // Exécution via clé interne (ordres limites, background service)
    private async Task ExecuteOrderCoreInternalAsync(Order order, decimal price)
    {
        decimal total = order.Quantity * price;

        if (order.Type == OrderType.Buy)
        {
            var balance = await _authClient.GetUserBalanceInternal(order.UserId);
            if (balance < total) throw new BadRequestException("Solde insuffisant.");

            await _authClient.UpdateUserBalanceInternal(order.UserId, -total);
            try
            {
                await _portfolioClient.UpdateHoldingInternalAsync(order.UserId, order.CryptoSymbol, order.Quantity, price, OrderType.Buy);
            }
            catch
            {
                await _authClient.UpdateUserBalanceInternal(order.UserId, total);
                throw;
            }
        }
        else
        {
            var stock = await _portfolioClient.GetHoldingQuantityInternalAsync(order.UserId, order.CryptoSymbol);
            if (stock < order.Quantity) throw new BadRequestException("Quantité insuffisante.");

            await _portfolioClient.UpdateHoldingInternalAsync(order.UserId, order.CryptoSymbol, order.Quantity, price, OrderType.Sell);
            await _authClient.UpdateUserBalanceInternal(order.UserId, total);
        }
    }
}
