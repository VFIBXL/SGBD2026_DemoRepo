using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DemoRepo.Models;
using DemoRepo.Repositories;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace TestProjectDemo
{
    public class AdoNetProductRepositoryTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _container = new MsSqlBuilder().Build();
        private IProductRepository _repo = null!;
        private string _cs = null!;

        public async Task InitializeAsync()
        {
            await _container.StartAsync();
            var cs = _cs = _container.GetConnectionString();

            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            await new SqlCommand(
                "CREATE TABLE Products (Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(100) CONSTRAINT UQ_Products_Name UNIQUE, Price DECIMAL(18,2))",
                conn).ExecuteNonQueryAsync();

            // CREATE PROCEDURE doit être seul dans son batch (pas de GO via SqlCommand)
            await new SqlCommand(
                """
                CREATE PROCEDURE [dbo].[Product_Delete]
                    @ProductId INT
                AS
                BEGIN
                    
                    DELETE FROM Products WHERE id = @ProductId
                END
                """,
                conn).ExecuteNonQueryAsync();

            // même chemin que Program.cs, seule la chaîne change
            _repo = ProductRepoFactory.CreateProductRepository(RepositoryType.AdoNet, cs);
        }

        public Task DisposeAsync() => _container.DisposeAsync().AsTask();

        [Fact]
        public void AddProduct_ThenGetById_ReturnsProduct()
        {
            int id = _repo.AddProduct(new Product { Name = "Test", Price = 9.99m });

            var p = _repo.GetProductById(id);
            Assert.NotNull(p);
            Assert.Equal("Test", p.Name);
            Assert.Equal(9.99m, p.Price);
        }


        [Fact]
        public void AddProduct_WithDuplicateName_ThrowsUniqueViolation()
        {
            _repo.AddProduct(new Product { Name = "Unique", Price = 1.00m });

            var ex = Assert.Throws<SqlException>(
                () => _repo.AddProduct(new Product { Name = "Unique", Price = 2.00m }));

            // 2627 = violation de contrainte UNIQUE / PRIMARY KEY
            Assert.Equal(2627, ex.Number);
            Assert.Contains("UQ_Products_Name", ex.Message);
        }

        [Fact]
        public void DeleteProduct_Existing_ReturnsTrueAndRemovesProduct()
        {
            int id = _repo.AddProduct(new Product { Name = "ToDelete", Price = 5.00m });

            bool deleted = _repo.DeleteProduct(id);

            Assert.True(deleted);
            Assert.Null(_repo.GetProductById(id));
        }

        [Fact]
        public void DeleteProduct_Unknown_ReturnsFalse()
        {
            bool deleted = _repo.DeleteProduct(9999);

            Assert.False(deleted);
        }
    }
}
