using AuthService.Dtos;
using AuthService.Models;

namespace AuthService.Extensions;

public static class MappingExtensions
{
    public static ProfilResponse ToProfilResponse(this User user)
    {
        return new ProfilResponse (
            Username: user.Username,
            Email: user.Email,
            Role: user.Role.ToString(),
            Balance: user.Balance,
            CreatedAt: user.CreatedAt
        );
    }
}