using Sample.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Application
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken ct = default);
        Task<int> AddProductAsync(Product product, CancellationToken ct = default);
        Task<bool> UpdateProductAsync(Product product, CancellationToken ct = default);
        Task<bool> DeleteProductAsync(int productId, CancellationToken ct = default);
        Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default);
    }
}
