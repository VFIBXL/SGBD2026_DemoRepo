using DemoRepo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRepo.Repositories
{
    public class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new()
            {
            new Product { Id = 1, Name = "Product 1", Price = 10.0m },
            new Product { Id = 2, Name = "Product 2", Price = 20.0m },
            new Product { Id = 3, Name = "Product 3", Price = 30.0m }
        };


        int IProductRepository.AddProduct(Product product)
        {
            _products.Add(product);
            return _products.Count + 1;
        }

        bool IProductRepository.DeleteProduct(int productId)
        {
            return _products.RemoveAll(p => p.Id == productId) > 0;
        }

        IEnumerable<Product> IProductRepository.GetAllProducts()
        {
            return _products;
        }

        Product? IProductRepository.GetProductById(int productId)
        {
            return _products.Find(p => p.Id == productId);
        }

        bool IProductRepository.UpdateProduct(Product product)
        {
            var existingProduct = _products.Find(p => p.Id == product.Id);
            if (existingProduct == null)
                return false;

            _products.Remove(existingProduct);
            _products.Add(product);
            return true;
        }

        void IProductRepository.ActionOnProduct(ActionEnum action)
        {
            // Implementation for handling actions on products
            switch(action)
            {
                case ActionEnum.GetAll:
                    // Call the explicit interface implementation to get all products
                    ((IProductRepository)this).GetAllProducts();
                    break;
                case ActionEnum.Add:
                    throw new NotImplementedException("Add action is not implemented in ActionOnProduct method.");
                case ActionEnum.Update:
                    throw new NotImplementedException("Update action is not implemented in ActionOnProduct method.");
                case ActionEnum.Delete:
                    throw new NotImplementedException("Delete action is not implemented in ActionOnProduct method.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
}
