using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;
using OrderProcessingSystem.Core.Services;
using OrderProcessingSystem.Core.Exceptions;

namespace OrderProcessingSystem.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<OrderService>>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _productServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task PlaceOrder_WithValidProducts_ShouldSucceed()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Price = 10.0m, StockQuantity = 5 };
            var quantity = 2;

            _productServiceMock.Setup(x => x.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _productServiceMock.Setup(x => x.UpdateStockAsync(productId, -quantity))
                .ReturnsAsync(true);

            var productQuantities = new Dictionary<Guid, int> { { productId, quantity } };

            // Act
            var result = await _orderService.PlaceOrderAsync(productQuantities);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(OrderStatus.Pending, result.Status);
            Assert.Single(result.Items);
            Assert.Equal(quantity * product.Price, result.TotalAmount);

            _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
            _productServiceMock.Verify(x => x.UpdateStockAsync(productId, -quantity), Times.Once);
        }

        [Fact]
        public async Task PlaceOrder_WithInsufficientStock_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Price = 10.0m, StockQuantity = 1 };
            var quantity = 2;

            _productServiceMock.Setup(x => x.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _productServiceMock.Setup(x => x.UpdateStockAsync(productId, -quantity))
                .ReturnsAsync(false);

            var productQuantities = new Dictionary<Guid, int> { { productId, quantity } };

            // Act & Assert
            await Assert.ThrowsAsync<InsufficientStockException>(() => 
                _orderService.PlaceOrderAsync(productQuantities));

            _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task PlaceOrder_WithNonExistentProduct_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var quantity = 1;

            _productServiceMock.Setup(x => x.GetByIdAsync(productId))
                .ReturnsAsync((Product)null);

            var productQuantities = new Dictionary<Guid, int> { { productId, quantity } };

            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(() => 
                _orderService.PlaceOrderAsync(productQuantities));

            _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task GetOrderById_WithExistingOrder_ShouldReturnOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId };

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
        }

        [Fact]
        public async Task GetOrderById_WithNonExistentOrder_ShouldThrowException()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act & Assert
            await Assert.ThrowsAsync<OrderNotFoundException>(() => 
                _orderService.GetOrderByIdAsync(orderId));
        }

        [Fact]
        public async Task CancelOrder_WithPendingOrder_ShouldSucceed()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = 2;
            var order = new Order();
            order.AddItem(new Product { Id = productId, Name = "Test Product", Price = 10.0m }, quantity);

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _productServiceMock.Setup(x => x.UpdateStockAsync(productId, quantity))
                .ReturnsAsync(true);

            // Act
            var result = await _orderService.CancelOrderAsync(orderId);

            // Assert
            Assert.True(result);
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            _orderRepositoryMock.Verify(x => x.UpdateAsync(order), Times.Once);
            _productServiceMock.Verify(x => x.UpdateStockAsync(productId, quantity), Times.Once);
        }

        [Fact]
        public async Task CancelOrder_WithNonPendingOrder_ShouldThrowException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order();
            order.Fulfill();

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _orderService.CancelOrderAsync(orderId));

            _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Order>()), Times.Never);
            _productServiceMock.Verify(x => x.UpdateStockAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        }
    }
}
