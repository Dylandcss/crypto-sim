namespace AuthService.DTOs;

public record ProfilResponse(
    string Username,
    string Email,
    string Role,
    decimal Balance,
    DateTime CreatedAt
);