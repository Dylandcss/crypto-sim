using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs;

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);