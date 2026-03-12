using PortfolioService.Models;
using PortfolioService.Dtos;

namespace PortfolioService.Extensions;

public static class MappingExtension
{
    public static TransactionDto ToTransactionDto(this Transaction transaction)
    {
        return new TransactionDto(
            transaction.Id,
            transaction.CryptoSymbol,
            transaction.Type,
            transaction.Quantity,
            transaction.PriceAtTime,
            transaction.Total,
            transaction.ExecutedAt
        );
    }
}