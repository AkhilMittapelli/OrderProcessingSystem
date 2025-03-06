using System;
using System.Linq;
using System.Collections.Generic;

namespace OrderProcessingSystem.Core.Entities
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public decimal TotalAmount => _items.Sum(item => item.Price * item.Quantity);

        public void AddItem(Product product, int quantity)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            }

            var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                _items.Add(new OrderItem(product.Id, product.Name, product.Price, quantity));
            }
        }

        public void Cancel()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException($"Cannot cancel order with status {Status}");
            }

            Status = OrderStatus.Cancelled;
        }

        public void Fulfill()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException($"Cannot fulfill order with status {Status}");
            }

            Status = OrderStatus.Fulfilled;
        }
    }

    public class OrderItem
    {
        public Guid ProductId { get; }
        public string ProductName { get; }
        public decimal Price { get; }
        public int Quantity { get; private set; }

        public OrderItem(Guid productId, string productName, decimal price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentException("Product name cannot be empty", nameof(productName));
            }

            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero", nameof(price));
            }

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            }

            ProductId = productId;
            ProductName = productName;
            Price = price;
            Quantity = quantity;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));
            }

            Quantity = newQuantity;
        }
    }

    public enum OrderStatus
    {
        Pending,
        Fulfilled,
        Cancelled
    }
}
