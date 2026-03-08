using System.ComponentModel.DataAnnotations;

namespace OrderService.Dtos;

public record AuthApiUpdateBalanceRequestDto
(
    [Required] decimal Amount
);
