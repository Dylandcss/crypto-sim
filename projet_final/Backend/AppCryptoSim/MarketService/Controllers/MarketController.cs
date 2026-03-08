using CryptoSim.Shared.Exceptions;
using MarketService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MarketController : ControllerBase
    {
        private readonly ICryptosService _cryptosService;
        private readonly IPriceHistoryService _priceHistoryService;

        public MarketController(ICryptosService cryptosService, IPriceHistoryService priceHistoryService)
        {
            _cryptosService = cryptosService;
            _priceHistoryService = priceHistoryService;
        }


        // GET /api/market/cryptos
        [HttpGet("cryptos")]
        public async Task<IActionResult> GetAllCryptosAsync()
        {
            var cryptos = await _cryptosService.GetAllCryptosAsync();
            return Ok(cryptos);
        }

        // GET /api/market/cryptos/{symbol}
        [HttpGet("cryptos/{symbol}")]
        public async Task<IActionResult> GetCryptoBySymbolAsync(string symbol) 
        {
            var crypto = await _cryptosService.GetCryptoBySymbolAsync(symbol);
            if (crypto == null) throw new NotFoundException($"La crypto {symbol} est introuvable.");
            return Ok(crypto);
        }

        // GET /api/market/history/{symbol}?limit=50
        [HttpGet("history/{symbol}")]
        public async Task<IActionResult> GetCryptoPriceHistoryAsync(string symbol, [FromQuery] int limit = 50)
        {
            if(limit <= 0) throw new ArgumentOutOfRangeException("La valeur de limit doit être superieure à 0");
            var result = await _priceHistoryService.GetPriceHistoryListAsync(symbol, limit);                  
            return Ok(result);
        }



    }
}
