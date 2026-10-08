using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRepo.Repositories
{
    public class ProductRepoFactory
    {
        public static IProductRepository CreateProductRepository(RepositoryType repositoryType , string? connectionString = null)
        {
            return repositoryType switch
            {
                RepositoryType.Fake => new FakeProductRepository(),
                RepositoryType.AdoNet => new AdoNetProductRepository(connectionString),
                _ => throw new ArgumentException("Invalid repository type", nameof(repositoryType)),
            };
        }
    }
}
