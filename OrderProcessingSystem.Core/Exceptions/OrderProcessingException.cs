using System;

namespace OrderProcessingSystem.Core.Exceptions
{
    public class OrderProcessingException : Exception
    {
        public OrderProcessingException(string message) : base(message)
        {
        }

        public OrderProcessingException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    public class InsufficientStockException : OrderProcessingException
    {
        public Guid ProductId { get; }
        public int RequestedQuantity { get; }
        public int AvailableQuantity { get; }

        public InsufficientStockException(Guid productId, int requestedQuantity, int availableQuantity)
            : base($"Insufficient stock for product {productId}. Requested: {requestedQuantity}, Available: {availableQuantity}")
        {
            ProductId = productId;
            RequestedQuantity = requestedQuantity;
            AvailableQuantity = availableQuantity;
        }
    }

    public class OrderNotFoundException : OrderProcessingException
    {
        public Guid OrderId { get; }

        public OrderNotFoundException(Guid orderId)
            : base($"Order {orderId} not found")
        {
            OrderId = orderId;
        }
    }

    public class ConcurrencyException : OrderProcessingException
    {
        public ConcurrencyException(string message) : base(message)
        {
        }
    }
}
