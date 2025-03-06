using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Core.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendOrderConfirmationAsync(Guid orderId)
        {
            _logger.LogInformation("Sending order confirmation for order {OrderId}", orderId);
            // Simulate sending email/notification
            await Task.Delay(100);
        }

        public async Task SendOrderFulfilledNotificationAsync(Guid orderId)
        {
            _logger.LogInformation("Sending order fulfillment notification for order {OrderId}", orderId);
            // Simulate sending email/notification
            await Task.Delay(100);
        }

        public async Task SendOrderCancellationNotificationAsync(Guid orderId)
        {
            _logger.LogInformation("Sending order cancellation notification for order {OrderId}", orderId);
            // Simulate sending email/notification
            await Task.Delay(100);
        }
    }
}
