using Microsoft.Data.SqlClient;
using Sample.Application;
using Sample.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Microsoft.Extensions.Options;

namespace Sample.Infrastructure
{
    public class AdoNetProductRepository : BaseRepo, IProductRepository
    {
        private readonly DatabaseOptions _options;
        private SqlConnection _connection;

        public AdoNetProductRepository(IOptions<DatabaseOptions> options)
        {
            _options = options.Value;
            
            if (string.IsNullOrEmpty(_options.ConnectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(_options.ConnectionString));
            }
            
            _connection = new SqlConnection(_options.ConnectionString);
        }

        public Task<int> AddProductAsync(Product product, CancellationToken ct = default)
        {
            string sql = LoadFileFromAssembly("AddProduct.sql");
            var command = new SqlCommand(sql, _connection);
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Price", product.Price);

            _connection.Open();

            int newProductId = (int)command.ExecuteScalar();

            _connection.Close();

            return Task.FromResult(newProductId);
        }

        public Task<bool> DeleteProductAsync(int productId, CancellationToken ct = default)
        {
            var command = new SqlCommand("Product_Delete", _connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ProductId", productId);

            _connection.Open();

            if (command.ExecuteNonQuery() > 0)
            {
                _connection.Close();
                return Task.FromResult(true);
            }

            _connection.Close();
            
            return Task.FromResult(false);
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken ct = default)
        {
            var products = new List<Product>();

            var sql = LoadFileFromAssembly("GetAllProducts.sql");

            var command = new SqlCommand(sql, _connection);
            command.CommandType = CommandType.Text;

            _connection.Open();

            Console.WriteLine(_connection.DataSource);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var product = new Product
                    (
                        reader.GetInt32("Id"),
                        reader.GetString("Name"),
                        reader.GetDecimal("Price")
                    );
                    //var product = new Product()
                    //{
                    //    Id = reader.GetInt32("Id"),
                    //    Name = reader.GetString("Name"),
                    //    Price = reader.GetDecimal("Price")
                    //};
                    products.Add(product);
                }
            }

            _connection.Close();

            return Task.FromResult<IEnumerable<Product>>(products);
        }

        public Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default)
        {
            Product? product = null;

            var sql = LoadFileFromAssembly("GetProductById.sql");
            var command = new SqlCommand(sql, _connection);
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@Id", productId);

            _connection.Open();

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    product = new Product
                    (
                        reader.GetInt32("Id"),
                        reader.GetString("Name"),
                        reader.GetDecimal("Price")
                    );
                }
            }
            _connection.Close();
            return Task.FromResult(product);
        }

        public Task<bool> UpdateProductAsync(Product product, CancellationToken ct = default)
        {
            var command = new SqlCommand("UPDATE Products SET Name = @Name, Price = @Price WHERE Id = @Id", _connection);
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Price", product.Price);

            _connection.Open();

            if (command.ExecuteNonQuery() > 0)
            {
                _connection.Close();
                return Task.FromResult(true);
            }
            _connection.Close();
            return Task.FromResult(false);
        }
    }
}
