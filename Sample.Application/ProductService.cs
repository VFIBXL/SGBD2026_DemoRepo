using Sample.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Application
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken ct = default)
        {
            var products = await _productRepository.GetAllProductsAsync(ct);
            return products.Select(p => new ProductDto(p.Id, p.Name, p.Price));
        }

        public async Task<int> AddProductAsync(CreateProductDto productDto, CancellationToken ct = default)
        {
            var product = new Product(productDto.Name, productDto.Price);

            return await _productRepository.AddProductAsync(product, ct);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken ct = default)
        {
            var product = await _productRepository.GetProductByIdAsync(productId, ct);
            if (product == null)
            {
                return null;
            }
            return new ProductDto(product.Id, product.Name, product.Price);
        }

        public async Task<bool> DeleteProductAsync(int productId, CancellationToken ct = default)
        {
            return await _productRepository.DeleteProductAsync(productId, ct);
        }

        public async Task<bool> UpdateProductAsync(int productId, UpdateProductDto productDto, CancellationToken ct = default)
        {
            var product = new Product(productId, productDto.Name, productDto.Price);
           
            return await _productRepository.UpdateProductAsync(product, ct);
        }
    }
}
