using AuthService.DTOs;

namespace AuthService.Extensions;

public static class MappingExtensions
{
    public static ProfilResponse ToProfilResponse(this Models.User user)
    {
        return new ProfilResponse(
            Username: user.Username,
            Email: user.Email,
            Role: user.Role.ToString(),
            Balance: user.Balance,
            CreatedAt: user.CreatedAt
        );
    }
}