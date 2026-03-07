using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos;

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);