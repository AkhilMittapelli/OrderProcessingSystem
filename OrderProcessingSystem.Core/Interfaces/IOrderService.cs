using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using OrderProcessingSystem.Core.Entities;

namespace OrderProcessingSystem.Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(Dictionary<Guid, int> productQuantities);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> CancelOrderAsync(Guid orderId);
        Task<bool> FulfillOrderAsync(Guid orderId);
        Task ProcessPendingOrdersAsync();
    }
}
