using System.ComponentModel.DataAnnotations;
using CryptoSim.Shared.Enums;


namespace OrderService.Dtos;

public record OrderRequest(
    [Required] string CryptoSymbol,
    [Required] OrderType Type,
    [Range(0.00000001, double.MaxValue)] decimal Quantity
);