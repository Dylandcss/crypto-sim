using System.ComponentModel.DataAnnotations;
using OrderService.Models;

namespace OrderService.Dtos;

public record OrderRequest(
    [Required] string CryptoSymbol,
    [Required] OrderType Type,
    [Range(0.00000001, double.MaxValue)] decimal Quantity
);