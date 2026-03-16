using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoSim.Shared.Exceptions;

public class UnauthorizedException : ExceptionBase
{
    public UnauthorizedException(string message) : base(message, 401)
    {
    }
}
