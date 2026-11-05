using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Application
{
    public record ProductDto(int Id, string Name, decimal Price);
    public record CreateProductDto(string Name, decimal Price);
    public record UpdateProductDto(int Id, string Name, decimal Price);
}
