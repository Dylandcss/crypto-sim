using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using CryptoSim.Shared.Enums;


namespace OrderService.Models;


[Index(nameof(UserId))] 
[Index(nameof(CreatedAt))]
public class Order
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(10)]
    public string CryptoSymbol { get; set; } = string.Empty;

    [Required]
    public OrderType Type { get; set; }

    [Required]
    [Precision(18, 8)]
    public decimal Quantity { get; set; }

    [Required]
    [Precision(18, 8)]
    public decimal Price { get; set; }

    [Required]
    [Precision(18, 8)]
    public decimal Total { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExecutedAt { get; set; }

    // Ordres limites
    public decimal? LimitPrice { get; set; }
}