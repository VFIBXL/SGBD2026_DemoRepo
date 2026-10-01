using System;
using System.Collections.Generic;
using System.Text;
using DemoRepo.Models;

namespace DemoRepo.Repositories
{
    interface IProductRepository
    {
        IEnumerable<Product> GetAllProducts();
        int AddProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(int productId);
        Product? GetProductById(int productId);
    }
}
