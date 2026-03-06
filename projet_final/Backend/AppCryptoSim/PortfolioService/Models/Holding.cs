namespace PortfolioService.Models
{
    public class Holding
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CryptoSymbol { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal AverageBuyPrice { get; set; }  // prix moyen d'achat
        public DateTime UpdatedAt { get; set; }
    }
}
