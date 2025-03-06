using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using OrderProcessingSystem.Core.Entities;

namespace OrderProcessingSystem.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(Guid id);
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<bool> UpdateStockAsync(Guid productId, int quantityChange);
    }
}