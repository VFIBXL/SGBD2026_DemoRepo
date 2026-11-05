using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Domain.Exceptions
{
    public class ProductNameRequiredException : DomainException
    {
        public ProductNameRequiredException() : base("Product name is required.") { }
    }
}
