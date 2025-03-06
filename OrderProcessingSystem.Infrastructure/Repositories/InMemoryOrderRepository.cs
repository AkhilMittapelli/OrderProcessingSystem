using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Repositories
{
    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<Guid, Order> _orders = new ConcurrentDictionary<Guid, Order>();

        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await Task.FromResult(_orders.TryGetValue(id, out var order) ? order : null);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await Task.FromResult(_orders.Values.AsEnumerable());
        }

        public async Task<Order> AddAsync(Order order)
        {
            if (!_orders.TryAdd(order.Id, order))
                throw new InvalidOperationException($"Order with ID {order.Id} already exists");
            
            return await Task.FromResult(order);
        }

        public async Task UpdateAsync(Order order)
        {
            _orders[order.Id] = order;
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            return await Task.FromResult(_orders.Values
                .Where(o => o.Status == OrderStatus.Pending)
                .AsEnumerable());
        }
    }
}
