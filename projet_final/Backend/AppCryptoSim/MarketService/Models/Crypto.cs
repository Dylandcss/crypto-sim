using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MarketService.Models;

public class Crypto
{
    
    public int Id { get; set; }
    
    [Required]
    [StringLength(10)]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Precision(18, 8)]
    public decimal CurrentPrice { get; set; }

    [Required]    
    public DateTime LastUpdated { get; set; }

    public ICollection<PriceHistory> History { get; set; } = new List<PriceHistory>();
}