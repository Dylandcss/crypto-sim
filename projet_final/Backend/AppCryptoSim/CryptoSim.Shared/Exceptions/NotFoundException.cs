using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoSim.Shared.Exceptions;

public class NotFoundException : ExceptionBase
{
    public NotFoundException(string message) : base(message, 404)
    {
    }
}
