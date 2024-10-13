using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskManagement.Common.Infrastructure.Exceptions;

public class DatabaseValidationException : Exception
{
    public DatabaseValidationException()
    {
    }

    public DatabaseValidationException(string? message) : base(message)
    {
    }

    public DatabaseValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
