namespace AuthService.Dtos;

public record AuthResponse(
    string Token,
    string Username,
    string Role,
    decimal Balance,
    long ExpiresIn
);