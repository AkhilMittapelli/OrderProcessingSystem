using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Repositories
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<Guid, Product> _products = new();

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_products.Values.AsEnumerable());
        }

        public Task<Product> GetByIdAsync(Guid id)
        {
            if (_products.TryGetValue(id, out var product))
            {
                return Task.FromResult(product);
            }
            return Task.FromResult<Product>(null);
        }

        public Task<Product> AddAsync(Product product)
        {
            if (!_products.TryAdd(product.Id, product))
            {
                throw new InvalidOperationException($"Product with ID {product.Id} already exists");
            }
            return Task.FromResult(product);
        }

        public Task<Product> UpdateAsync(Product product)
        {
            _products[product.Id] = product;
            return Task.FromResult(product);
        }

        public Task DeleteAsync(Guid id)
        {
            _products.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}
