using CryptoSim.Shared.Enums;
using CryptoSim.Shared.Exceptions;
using PortfolioService.Services.Clients;
using PortfolioService.Dtos;
using PortfolioService.Models;
using PortfolioService.Repositories;

namespace PortfolioService.Services;

public class PortfolioManagementService : IPortfolioManagementService
{
    private readonly IPortfolioRepository _repository;
    private readonly MarketApiClient _marketApiClient;

    public PortfolioManagementService(
        IPortfolioRepository repository,
        MarketApiClient marketApiClient)
    {
        _repository = repository;
        _marketApiClient = marketApiClient;
    }

    public async Task<PortfolioSummary> GetPortfolioSummaryAsync(int userId, string token)
    {
        var holdings = await _repository.GetUserHoldingsAsync(userId);

        var holdingDetails = new List<HoldingDetail>();

        decimal totalInvested = 0;
        decimal totalCurrentValue = 0;

        foreach (var holding in holdings)
        {
            var currentPrice = await _marketApiClient.GetCryptoPriceAsync(holding.CryptoSymbol, token);
            var currentValue = currentPrice * holding.Quantity;
            var invested = holding.AverageBuyPrice * holding.Quantity;

            var gainLoss = currentValue - invested;
            var gainLossPercent = holding.AverageBuyPrice == 0
                ? 0
                : ((currentPrice - holding.AverageBuyPrice) / holding.AverageBuyPrice) * 100;

            holdingDetails.Add(new HoldingDetail(
                holding.CryptoSymbol,
                holding.CryptoSymbol,
                holding.Quantity,
                holding.AverageBuyPrice,
                currentPrice,
                currentValue,
                gainLoss,
                gainLossPercent
            ));

            totalInvested += invested;
            totalCurrentValue += currentValue;
        }

        var totalGainLoss = totalCurrentValue - totalInvested;

        var totalGainLossPercent = totalInvested == 0
            ? 0
            : (totalGainLoss / totalInvested) * 100;

        return new PortfolioSummary(
            userId,
            0,
            totalInvested,
            totalCurrentValue,
            totalGainLoss,
            totalGainLossPercent,
            holdingDetails
        );
    }

    public async Task<List<HoldingDetail>> GetHoldingsAsync(int userId, string token)
    {
        var holdings = await _repository.GetUserHoldingsAsync(userId);
        var result = new List<HoldingDetail>();

        foreach (var holding in holdings)
        {
            var price = await _marketApiClient.GetCryptoPriceAsync(holding.CryptoSymbol, token);

            var currentValue = price * holding.Quantity;
            var gainLoss = (price - holding.AverageBuyPrice) * holding.Quantity;

            var gainLossPercent = holding.AverageBuyPrice == 0
                ? 0
                : ((price - holding.AverageBuyPrice) / holding.AverageBuyPrice) * 100;

            result.Add(new HoldingDetail(
                holding.CryptoSymbol,
                holding.CryptoSymbol,
                holding.Quantity,
                holding.AverageBuyPrice,
                price,
                currentValue,
                gainLoss,
                gainLossPercent
            ));
        }

        return result;
    }

    public async Task<HoldingDetail?> GetHoldingAsync(int userId, string symbol, string token)
    {
        var holding = await _repository.GetHoldingAsync(userId, symbol);
        if (holding == null) throw new NotFoundException("Holding not found");
        
        var price = await _marketApiClient.GetCryptoPriceAsync(holding.CryptoSymbol, token);
        var currentValue = price * holding.Quantity;
        var gainLoss = (price - holding.AverageBuyPrice) * holding.Quantity;
        var gainLossPercent = holding.AverageBuyPrice == 0
            ? 0
            : ((price - holding.AverageBuyPrice) / holding.AverageBuyPrice) * 100;
        
        return new HoldingDetail(holding.CryptoSymbol, holding.CryptoSymbol, holding.Quantity, holding.AverageBuyPrice, price, currentValue, gainLoss, gainLossPercent);
    }

    public async Task<List<Transaction>> GetTransactionsAsync(int userId)
    {
        return await _repository.GetUserTransactionsAsync(userId);
    }

    public async Task<PortfolioPerformanceDto> GetPerformanceAsync(int userId, string token)
    {
        var summary = await GetPortfolioSummaryAsync(userId, token);

        return new PortfolioPerformanceDto(
            summary.TotalInvested,
            summary.TotalCurrentValue,
            summary.TotalGainLoss,
            summary.TotalGainLossPercent
        );
    }

    public async Task UpdatePortfolioAfterTradeAsync(
        int userId,
        string symbol,
        OrderType type,
        decimal quantity,
        decimal price)
    {
        var holding = await _repository.GetHoldingAsync(userId, symbol);

        if (type == OrderType.Buy)
        {
            if (holding == null)
            {
                holding = new Holding
                {
                    UserId = userId,
                    CryptoSymbol = symbol,
                    Quantity = quantity,
                    AverageBuyPrice = price,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repository.AddHoldingAsync(holding);
            }
            else
            {
                var totalCost =
                    (holding.Quantity * holding.AverageBuyPrice) +
                    (quantity * price);

                holding.Quantity += quantity;
                holding.AverageBuyPrice = totalCost / holding.Quantity;
                holding.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateHoldingAsync(holding);
            }
        }
        else if (type == OrderType.Sell)
        {
            if (holding == null || holding.Quantity < quantity)
                throw new Exception("Not enough crypto to sell");

            holding.Quantity -= quantity;

            if (holding.Quantity == 0)
            {
                holding.AverageBuyPrice = 0;
            }

            holding.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateHoldingAsync(holding);
        }

        var transaction = new Transaction()
        {
            UserId = userId,
            CryptoSymbol = symbol,
            Type = type,
            Quantity = quantity,
            PriceAtTime = price,
            Total = quantity * price,
            ExecutedAt = DateTime.UtcNow
        };

        await _repository.AddTransactionAsync(transaction);
    }
}