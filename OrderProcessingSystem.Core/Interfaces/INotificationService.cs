using System;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Core.Interfaces
{
    public interface INotificationService
    {
        Task SendOrderConfirmationAsync(Guid orderId);
        Task SendOrderFulfilledNotificationAsync(Guid orderId);
        Task SendOrderCancellationNotificationAsync(Guid orderId);
    }
}
