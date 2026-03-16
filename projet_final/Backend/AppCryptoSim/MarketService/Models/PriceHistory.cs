using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace MarketService.Models;


[Index(nameof(CryptoSymbol))]
[Index(nameof(RecordedAt))]
public class PriceHistory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(10)]
    public string CryptoSymbol { get; set; } = string.Empty;

    [Required]
    [Precision(18, 8)]
    public decimal Price { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime RecordedAt { get; set; }
}
