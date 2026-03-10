using CryptoSim.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioService.Dtos;
using PortfolioService.Models;
using PortfolioService.Services;
using CryptoSim.Shared.Extensions;

namespace PortfolioService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PortfolioController : ControllerBase
{
        private readonly IPortfolioManagementService _portfolioService;

        public PortfolioController(IPortfolioManagementService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        /// <summary>
        /// Récupère le résumé complet du portefeuille pour l'utilisateur connecté
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PortfolioSummary>> GetPortfolioSummary()
        {
            var userId = User.GetUserId();
            var token = HttpContext.GetBearerToken();

            var summary = await _portfolioService.GetPortfolioSummaryAsync(userId, token);
            return Ok(summary);
        }

        /// <summary>
        /// Récupère la liste des actifs détenus par l'utilisateur
        /// </summary>
        [HttpGet("holdings")]
        public async Task<ActionResult<List<HoldingDetail>>> GetHoldings()
        {
            var userId = User.GetUserId();
            var token = HttpContext.GetBearerToken();

            var holdings = await _portfolioService.GetHoldingsAsync(userId, token);
            return Ok(holdings);
        }
        
        /// <summary>
        /// Récupère le détail d'un actif spécifique détenu par l'utilisateur
        /// </summary>
        [HttpGet("holdings/{symbol}")]
        public async Task<ActionResult<List<HoldingDetail>>> GetHolding(string symbol)
        {
            var userId = User.GetUserId();
            var token = HttpContext.GetBearerToken();

            var holding = await _portfolioService.GetHoldingAsync(userId, symbol, token);
            return Ok(holding);
        }

        /// <summary>
        /// Récupère l'historique des transactions de l'utilisateur
        /// </summary>
        [HttpGet("transactions")]
        public async Task<ActionResult<List<Transaction>>> GetTransactions()
        {
            var userId = User.GetUserId();

            var transactions = await _portfolioService.GetTransactionsAsync(userId);
            return Ok(transactions);
        }

        /// <summary>
        /// Récupère la performance globale du portefeuille (gain/perte total)
        /// </summary>
        [HttpGet("performance")]
        public async Task<ActionResult<PortfolioPerformanceDto>> GetPerformance()
        {
            var userId = User.GetUserId();
            var token = HttpContext.GetBearerToken();

            var performance = await _portfolioService.GetPerformanceAsync(userId, token);
            return Ok(performance);
        }

        [HttpPost("holdings")]
        public async Task<ActionResult> UpdatePortfolioAfterTrade([FromBody] UpdatePortfolioDto dto)
        {
            var userId = User.GetUserId();

            await _portfolioService.UpdatePortfolioAfterTradeAsync(userId, dto.CryptoSymbol, dto.Type, dto.Quantity, dto.Price);
            return Ok();
        }
}