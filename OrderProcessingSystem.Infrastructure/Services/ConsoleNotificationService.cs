using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Core.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Services
{
    public class ConsoleNotificationService : INotificationService
    {
        private readonly ILogger<ConsoleNotificationService> _logger;

        public ConsoleNotificationService(ILogger<ConsoleNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendOrderConfirmationAsync(Guid orderId)
        {
            _logger.LogInformation("Order {OrderId} has been confirmed", orderId);
            await Task.CompletedTask;
        }

        public async Task SendOrderFulfilledNotificationAsync(Guid orderId)
        {
            _logger.LogInformation("Order {OrderId} has been fulfilled", orderId);
            await Task.CompletedTask;
        }

        public async Task SendOrderCancellationNotificationAsync(Guid orderId)
        {
            _logger.LogInformation("Order {OrderId} has been cancelled", orderId);
            await Task.CompletedTask;
        }
    }
}
