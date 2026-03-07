using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos;

public record UpdateBalanceRequest(
    
    [Required]
    decimal Amount

 );
