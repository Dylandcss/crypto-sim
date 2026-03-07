using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoSim.Shared.Exceptions;

public abstract class ExceptionBase : Exception
{

    public int StatusCode { get; set; }

    public ExceptionBase(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }


}
