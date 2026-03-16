using CryptoSim.Shared.Exceptions;

namespace AuthService.Exceptions;

public class UserAlreadyExistsException : BadRequestException
{
    public UserAlreadyExistsException(string username) : base($"L'utilisateur '{username}' existe déjà.")
    {
    }
}