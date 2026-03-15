using CryptoSim.Shared.Constants;
using CryptoSim.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioService.Dtos;
using PortfolioService.Models;
using PortfolioService.Services;

namespace PortfolioService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PortfolioController : ControllerBase
{
        private readonly IPortfolioManagementService _portfolioService;
        private readonly IConfiguration _config;

        public PortfolioController(IPortfolioManagementService portfolioService, IConfiguration config)
        {
            _portfolioService = portfolioService;
            _config = config;
        }

        private bool IsInternalRequest() =>
            Request.Headers.TryGetValue("X-Internal-Key", out var key) &&
            key == _config[EnvConstants.InternalApiKey];

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

            await _portfolioService.UpdatePortfolioAfterTradeAsync(userId, dto.CryptoSymbol, dto.Type, dto.Quantity, dto.PriceAtTime);
            return Ok();
        }

        // --- Endpoints internes (appelés par OrderService via X-Internal-Key) ---

        [HttpGet("internal/holdings/{symbol}/quantity/{userId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHoldingQuantityInternalAsync(string symbol, int userId)
        {
            if (!IsInternalRequest()) return Unauthorized();
            var holding = await _portfolioService.GetHoldingAsync(userId, symbol, "");
            return Ok(new { CryptoSymbol = symbol, Quantity = holding?.Quantity ?? 0 });
        }

        [HttpPost("internal/holdings")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdatePortfolioInternalAsync([FromBody] InternalUpdatePortfolioDto dto)
        {
            if (!IsInternalRequest()) return Unauthorized();
            await _portfolioService.UpdatePortfolioAfterTradeAsync(dto.UserId, dto.CryptoSymbol, dto.Type, dto.Quantity, dto.PriceAtTime);
            return Ok();
        }
}