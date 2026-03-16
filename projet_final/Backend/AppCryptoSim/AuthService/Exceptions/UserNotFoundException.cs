using CryptoSim.Shared.Exceptions;

namespace AuthService.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(int userId) : base($"L'utilisateur avec l'id {userId} n'a pas été trouvé.")
    {
    }
}