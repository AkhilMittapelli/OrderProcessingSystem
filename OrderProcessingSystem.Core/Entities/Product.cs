using System;

namespace OrderProcessingSystem.Core.Entities
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public bool TryReserveStock(int quantity)
        {
            if (quantity <= 0) return false;
            if (StockQuantity < quantity) return false;

            StockQuantity -= quantity;
            return true;
        }

        public void RestoreStock(int quantity)
        {
            if (quantity > 0)
            {
                StockQuantity += quantity;
            }
        }

        public void UpdateStock(int newQuantity)
        {
            if (newQuantity >= 0)
            {
                StockQuantity = newQuantity;
            }
        }
    }
}
