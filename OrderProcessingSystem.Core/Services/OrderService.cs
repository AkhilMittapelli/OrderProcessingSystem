using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;
using OrderProcessingSystem.Core.Exceptions;

namespace OrderProcessingSystem.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly ILogger<OrderService> _logger;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public OrderService(
            IOrderRepository orderRepository,
            IProductService productService,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Order> PlaceOrderAsync(Dictionary<Guid, int> productQuantities)
        {
            if (productQuantities == null || !productQuantities.Any())
            {
                throw new ValidationException("Order must contain at least one product");
            }

            await _semaphore.WaitAsync();
            try
            {
                var order = new Order();

                foreach (var (productId, quantity) in productQuantities)
                {
                    var product = await _productService.GetByIdAsync(productId);
                    if (product == null)
                    {
                        throw new ProductNotFoundException($"Product with ID {productId} not found");
                    }

                    if (!await _productService.UpdateStockAsync(productId, -quantity))
                    {
                        throw new InsufficientStockException(productId, quantity, product.StockQuantity);
                    }

                    order.AddItem(product, quantity);
                }

                await _orderRepository.AddAsync(order);
                _logger.LogInformation("Order {OrderId} placed successfully", order.Id);
                return order;
            }
            catch (Exception)
            {
                _logger.LogError("Failed to place order");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new OrderNotFoundException(orderId);
            }
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<bool> CancelOrderAsync(Guid orderId)
        {
            await _semaphore.WaitAsync();
            try
            {
                var order = await GetOrderByIdAsync(orderId);

                if (order.Status != OrderStatus.Pending)
                {
                    throw new InvalidOperationException($"Cannot cancel order {orderId} with status {order.Status}");
                }

                foreach (var item in order.Items)
                {
                    await _productService.UpdateStockAsync(item.ProductId, item.Quantity);
                }

                order.Cancel();
                await _orderRepository.UpdateAsync(order);
                _logger.LogInformation("Order {OrderId} cancelled successfully", orderId);
                return true;
            }
            catch (Exception)
            {
                _logger.LogError("Failed to cancel order {OrderId}", orderId);
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> FulfillOrderAsync(Guid orderId)
        {
            await _semaphore.WaitAsync();
            try
            {
                var order = await GetOrderByIdAsync(orderId);

                if (order.Status != OrderStatus.Pending)
                {
                    throw new InvalidOperationException($"Cannot fulfill order {orderId} with status {order.Status}");
                }

                order.Fulfill();
                await _orderRepository.UpdateAsync(order);
                _logger.LogInformation("Order {OrderId} fulfilled successfully", orderId);
                return true;
            }
            catch (Exception)
            {
                _logger.LogError("Failed to fulfill order {OrderId}", orderId);
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task ProcessPendingOrdersAsync()
        {
            var pendingOrders = (await _orderRepository.GetAllAsync())
                .Where(o => o.Status == OrderStatus.Pending)
                .ToList();

            foreach (var order in pendingOrders)
            {
                try
                {
                    await FulfillOrderAsync(order.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process pending order {OrderId}", order.Id);
                }
            }
        }
    }
}
