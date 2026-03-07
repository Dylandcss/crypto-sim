using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos;

public record RegisterRequest(
    [Required][MinLength(3)] string Username,
    [Required][MinLength(6)] string Password,
    [Required][EmailAddress] string Email
);