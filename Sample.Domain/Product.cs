using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Domain
{
    public class Product
    {
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name
        {
            get { 
                return _name; 
            }
            //set { _Name = value ?? string.Empty; }
        }

        public decimal Price { get; set; }

        private string _name = string.Empty;

        public Product(int id, string name, decimal price) : this(name, price)
        {
            Id = id;
        }

        public Product(string name, decimal price)
        {
            ValidName(name);
            _name = name;

            Price = price;
        }

        public override string ToString()
        {
            return $"Product Id: {Id}, Name: {Name}, Price: {Price:C}";
        }

        private void ValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exceptions.ProductNameRequiredException();
            }
        }
    }
}
