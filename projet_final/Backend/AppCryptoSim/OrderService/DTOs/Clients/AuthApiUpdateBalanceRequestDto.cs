using System.ComponentModel.DataAnnotations;

namespace OrderService.Dtos.Clients;

public record AuthApiUpdateBalanceRequestDto
(
    [Required] decimal Amount
);
