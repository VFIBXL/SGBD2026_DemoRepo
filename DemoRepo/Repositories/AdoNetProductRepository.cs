using DemoRepo.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DemoRepo.Repositories
{
    public class AdoNetProductRepository : IProductRepository
    {
        private SqlConnection _connection;
        //private IDbConnection dbConnection;

        public AdoNetProductRepository(string? connectionString)
        {
            // Initialize the repository, e.g., set up database connection
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }   
            _connection = new SqlConnection(connectionString);
        }

        string LoadFileFromAssembly(string fileName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var manifestResourceNames = assembly.GetManifestResourceNames();

            string resourceName = manifestResourceNames
                .FirstOrDefault(name => name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                ?? throw new FileNotFoundException($"Embedded resource '{fileName}' not found.");

            using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        int IProductRepository.AddProduct(Product product)
        {
            string sql = LoadFileFromAssembly("AddProduct.sql");
            var command = new SqlCommand(sql, _connection);
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Price", product.Price);
            
            _connection.Open();
            
            int newProductId = (int)command.ExecuteScalar();
            
            _connection.Close();
            
            return newProductId;
        }

        bool IProductRepository.DeleteProduct(int productId)
        {
            //var sql = LoadFileFromAssembly("DeleteProduct.sql");
            
            var command = new SqlCommand("Product_Delete", _connection);
            command.CommandType = CommandType.StoredProcedure;
            
            command.Parameters.AddWithValue("@ProductId", productId);
            
            _connection.Open();

            if (command.ExecuteNonQuery() > 0)
            {
                _connection.Close();
                return true;
            }

            _connection.Close();
            return false;
        }

        IEnumerable<Product> IProductRepository.GetAllProducts()
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
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Price = reader.GetDecimal("Price")
                    };
                    products.Add(product);
                }
            }

            _connection.Close();

            return products;
        }
        

        Product? IProductRepository.GetProductById(int productId)
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
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Price = reader.GetDecimal("Price")
                    };
                }
            }
            _connection.Close();
            return product;
        }

        bool IProductRepository.UpdateProduct(Product product)
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
                return true;
            }
            _connection.Close();
            return false;
        }

        void IProductRepository.ActionOnProduct(ActionEnum action)
        {
            // Implementation for handling actions on products
            throw new NotImplementedException();    
        }
    }
}
