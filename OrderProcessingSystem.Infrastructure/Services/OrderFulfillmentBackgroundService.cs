using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Core.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Services
{
    public class OrderFulfillmentBackgroundService : BackgroundService
    {
        private readonly ILogger<OrderFulfillmentBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public OrderFulfillmentBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<OrderFulfillmentBackgroundService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Order Fulfillment Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    await orderService.ProcessPendingOrdersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing pending orders");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
