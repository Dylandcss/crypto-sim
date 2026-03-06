using System.ComponentModel.DataAnnotations;


namespace MarketService.Models;

public class Crypto
{
    
    public int Id { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]  
    public decimal CurrentPrice { get; set; }

    [Required]    
    public DateTime LastUpdated { get; set; }
}