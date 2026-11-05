using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sample.Infrastructure
{
    public class DatabaseOptions
    {
        [Required(ErrorMessage = "Connection string is required.")]
        public string ConnectionString { get; set; }

        [Range(1, 600, ErrorMessage = "Command timeout must be between 1 and 600 seconds.")]
        public int CommandTimeout { get; set; } = 30; // Default command timeout in seconds
    }
}
