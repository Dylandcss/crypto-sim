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
    private readonly AuthApiClient _authApiClient;

    public PortfolioManagementService(
        IPortfolioRepository repository,
        MarketApiClient marketApiClient,
        AuthApiClient authApiClient)
    {
        _repository = repository;
        _marketApiClient = marketApiClient;
        _authApiClient = authApiClient;
    }

    public async Task<PortfolioSummary> GetPortfolioSummaryAsync(int userId, string token)
    {
        var holdings = await _repository.GetUserHoldingsAsync(userId);
        var transactions = await _repository.GetUserTransactionsAsync(userId);

        var holdingDetails = new List<HoldingDetail>();

        var balance = await _authApiClient.GetUserBalance(token);
        var marketCryptoPrices = await _marketApiClient.GetCryptosPricesAsync(token);

        // Calcul basé sur les transactions
        decimal totalBought = transactions
            .Where(t => t.Type == OrderType.Buy)
            .Sum(t => t.Total);

        decimal totalSold = transactions
            .Where(t => t.Type == OrderType.Sell)
            .Sum(t => t.Total);

        // Valeur actuelle des holdings encore détenus
        decimal totalCurrentValue = 0;
        decimal unrealizedGainLoss = 0;

        foreach (var holding in holdings)
        {
            var currentPrice = marketCryptoPrices.FirstOrDefault(c => c.Symbol == holding.CryptoSymbol)?.CurrentPrice ?? 0m;

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

            totalCurrentValue += currentValue;
            unrealizedGainLoss += gainLoss;
        }

        // TotalInvested = tout ce qui a été acheté au total (basé sur les transactions)
        decimal totalInvested = totalBought;

        // Gains réalisés = total des ventes - coût d'acquisition des cryptos vendues
        decimal realizedGainLoss = totalSold - (totalBought - holdings.Sum(h => h.AverageBuyPrice * h.Quantity));

        // Gain/Perte total = réalisé + non réalisé
        decimal totalGainLoss = realizedGainLoss + unrealizedGainLoss;

        decimal totalGainLossPercent = totalInvested == 0 ? 0 : (totalGainLoss / totalInvested) * 100;

        return new PortfolioSummary(
            userId,
            balance,
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

        var marketCryptoPrices = await _marketApiClient.GetCryptosPricesAsync(token);

        foreach (var holding in holdings)
        {
            var price = marketCryptoPrices.FirstOrDefault(c => c.Symbol == holding.CryptoSymbol)?.CurrentPrice ?? 0m;

            var currentValue = price * holding.Quantity;
            var gainLoss = (price - holding.AverageBuyPrice) * holding.Quantity;

            var gainLossPercent = holding.AverageBuyPrice == 0 ? 0 : ((price - holding.AverageBuyPrice) / holding.AverageBuyPrice) * 100;

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

    public async Task<List<TransactionDto>> GetTransactionsAsync(int userId)
    {
        var transactions = await _repository.GetUserTransactionsAsync(userId);
        return transactions.Select(t => new TransactionDto(
            t.Id,
            t.CryptoSymbol,
            t.Type,
            t.Quantity,
            t.PriceAtTime,
            t.Total,
            t.ExecutedAt
        )).ToList();
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
                await _repository.DeleteHoldingAsync(holding);
            }
            else
            {
                holding.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateHoldingAsync(holding);
            }
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