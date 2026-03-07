namespace AuthService.Dtos;

public record ProfilResponse(
    string Username,
    string Email,
    string Role,
    decimal Balance,
    DateTime CreatedAt
);