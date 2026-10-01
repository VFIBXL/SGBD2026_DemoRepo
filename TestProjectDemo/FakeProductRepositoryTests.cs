using DemoRepo.Repositories;
using DemoRepo.Models;
using System.Linq;
using Xunit;

namespace TestProjectDemo
{
    public class FakeProductRepositoryTests
    {
        private readonly DemoRepo.Repositories.IProductRepository _productRepository;

        public FakeProductRepositoryTests()
        {
            _productRepository = new DemoRepo.Repositories.FakeProductRepository();
        }

        [Fact]
        public void GetAll_ReturnsAllProducts()
        {
            var products = _productRepository.GetAllProducts();
            Assert.Equal(3, products.Count());
        }

        [Fact]
        public void GetById_ExistingId_ReturnsProduct()
        {
            var product = _productRepository.GetProductById(1);
            Assert.NotNull(product);
            Assert.Equal(1, product.Id);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetByIds_ExistingId_ReturnsProduct(int productId)
        {
            var product = _productRepository.GetProductById(productId);
            Assert.NotNull(product);
            Assert.Equal(productId, product.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNull()
        {
            var product = _productRepository.GetProductById(999);
            Assert.Null(product);
        }

        [Fact]
        public void AddProduct_AddsProductSuccessfully()
        {
            var newProduct = new DemoRepo.Models.Product { Id = 4, Name = "New Product", Price = 49.99m };
            int newProductId = _productRepository.AddProduct(newProduct);
            Assert.Equal(5, newProductId); // Since there were initially 3 products, the next ID should be 5
            var addedProduct = _productRepository.GetProductById(4);
            Assert.NotNull(addedProduct);
            Assert.Equal("New Product", addedProduct.Name);
        }

        [Fact]
        public void UpdateProduct_ExistingProduct_UpdatesSuccessfully()
        {
            var productToUpdate = _productRepository.GetProductById(1);
            Assert.NotNull(productToUpdate);
            productToUpdate.Name = "Updated Product";
            bool result = _productRepository.UpdateProduct(productToUpdate);
            Assert.True(result);
            var updatedProduct = _productRepository.GetProductById(1);
            Assert.Equal("Updated Product", updatedProduct.Name);
        }

        [Fact]
        public void UpdateProduct_NonExistingProduct_ReturnsFalse()
        {
            var nonExistingProduct = new DemoRepo.Models.Product { Id = 999, Name = "Non-Existing Product", Price = 99.99m };
            bool result = _productRepository.UpdateProduct(nonExistingProduct);
            Assert.False(result);
        }

        [Fact]
        public void DeleteProduct_ExistingProduct_DeletesSuccessfully()
        {
            IProductRepository productRepository = new DemoRepo.Repositories.FakeProductRepository(); // Reset repository to initial state
            var productToDelete = productRepository.GetProductById(1);
            Assert.NotNull(productToDelete);
            bool result = productRepository.DeleteProduct(1);
            Assert.True(result);
            var deletedProduct = productRepository.GetProductById(1);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public void DeleteProduct_NonExistingProduct_ReturnsFalse()
        {
            IProductRepository productRepository = new DemoRepo.Repositories.FakeProductRepository(); // Reset repository to initial state
            bool result = productRepository.DeleteProduct(999);
            Assert.False(result);
        }

        [Fact]
        public void ActionOnProduct_GetAll_ReturnsAllProducts()
        {
            IProductRepository productRepository = new DemoRepo.Repositories.FakeProductRepository(); // Reset repository to initial state
            productRepository.ActionOnProduct(ActionEnum.GetAll);
        }

        [Theory]
        [InlineData(ActionEnum.Add, "Add action is not implemented")]
        [InlineData(ActionEnum.Update, "Update action is not implemented")]
        [InlineData(ActionEnum.Delete, "Delete action is not implemented")]
        public void ActionOnProduct_ThrowsNotImplementedExceptionAndIncorrectMessage(ActionEnum action , string message)
        {
            IProductRepository productRepository = new DemoRepo.Repositories.FakeProductRepository(); // Reset repository to initial state
            var exception = Assert.Throws<NotImplementedException>(() => productRepository.ActionOnProduct(action));
            Assert.NotEqual(message, exception.Message);
        }

        [Theory]
        [InlineData(ActionEnum.Add, "Add action is not implemented in ActionOnProduct method.")]
        [InlineData(ActionEnum.Update, "Update action is not implemented in ActionOnProduct method.")]
        [InlineData(ActionEnum.Delete, "Delete action is not implemented in ActionOnProduct method.")]
        public void ActionOnProduct_ThrowsNotImplementedExceptionAndCorrectMessage(ActionEnum action, string message)
        {
            IProductRepository productRepository = new DemoRepo.Repositories.FakeProductRepository(); // Reset repository to initial state
            var exception = Assert.Throws<NotImplementedException>(() => productRepository.ActionOnProduct(action));
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void ActionOnProduct_ArgumentOutOfRangeException()
        {
            IProductRepository productRepository = new DemoRepo.Repositories.FakeProductRepository(); // Reset repository to initial state
            Assert.Throws<ArgumentOutOfRangeException>(() => productRepository.ActionOnProduct((ActionEnum)999));
        }
    }
}
