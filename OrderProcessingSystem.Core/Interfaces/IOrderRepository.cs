using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderProcessingSystem.Core.Entities;

namespace OrderProcessingSystem.Core.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task<IEnumerable<Order>> GetPendingOrdersAsync();
    }
}
