using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs;

public record UpdateBalanceRequest(
    [Required]
    string Balance
 );
