using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos;

public record LoginRequest (
    [Required][MinLength(3, ErrorMessage = "Le nom d'utilisateur doit contenir au moins 3 caractères.")] string Username,
    [Required][MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")] string Password
);