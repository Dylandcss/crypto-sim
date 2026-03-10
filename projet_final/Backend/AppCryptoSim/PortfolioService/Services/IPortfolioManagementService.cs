using CryptoSim.Shared.Enums;
using PortfolioService.Dtos;
using PortfolioService.Models;

namespace PortfolioService.Services;

public interface IPortfolioManagementService
{
    // Résumé complet du portefeuille
    Task<PortfolioSummary> GetPortfolioSummaryAsync(int userId, string token);

    // Liste des holdings
    Task<List<HoldingDetail>> GetHoldingsAsync(int userId, string token);
    
    // Détail d'un holding spécifique
    Task<HoldingDetail?> GetHoldingAsync(int userId, string cryptoSymbol, string token);

    // Historique des transactions
    Task<List<Transaction>> GetTransactionsAsync(int userId);

    // Performance globale
    Task<PortfolioPerformanceDto> GetPerformanceAsync(int userId, string token);
    
    // Mise à jour du portefeuille après exécution d'un trade
    Task UpdatePortfolioAfterTradeAsync(
        int userId,
        string cryptoSymbol,
        OrderType type,
        decimal quantity,
        decimal price
    );
}