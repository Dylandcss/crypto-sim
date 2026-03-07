using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoSim.Shared.Exceptions;

public class BadRequestException : ExceptionBase
{
    public BadRequestException(string message) : base(message, 400)
    {
    }
}
