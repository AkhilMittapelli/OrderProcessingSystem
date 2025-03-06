using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Infrastructure.Data;

namespace OrderProcessingSystem.Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrderProcessingDbContext _context;

        public ProductRepository(OrderProcessingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            product.Id = Guid.NewGuid();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
