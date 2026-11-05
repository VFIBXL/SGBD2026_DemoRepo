using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
    }
}
