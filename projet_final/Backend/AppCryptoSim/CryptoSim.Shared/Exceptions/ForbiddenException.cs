namespace CryptoSim.Shared.Exceptions;

public class ForbiddenException : ExceptionBase
{
    public ForbiddenException(string message) : base(message, 403)
    {
    }
}
