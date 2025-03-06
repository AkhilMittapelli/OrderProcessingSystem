using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;
using OrderProcessingSystem.Core.Exceptions;

namespace OrderProcessingSystem.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await _productRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                throw;
            }
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    throw new ProductNotFoundException($"Product with ID {id} not found");
                }
                return product;
            }
            catch (Exception ex) when (ex is not ProductNotFoundException)
            {
                _logger.LogError(ex, "Error retrieving product {ProductId}", id);
                throw;
            }
        }

        public async Task<Product> AddAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            try
            {
                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    throw new ValidationException("Product name is required");
                }

                if (product.Price < 0)
                {
                    throw new ValidationException("Product price cannot be negative");
                }

                if (product.StockQuantity < 0)
                {
                    throw new ValidationException("Product stock quantity cannot be negative");
                }

                return await _productRepository.AddAsync(product);
            }
            catch (Exception ex) when (ex is not ValidationException)
            {
                _logger.LogError(ex, "Error creating product {@Product}", product);
                throw;
            }
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            try
            {
                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    throw new ValidationException("Product name is required");
                }

                if (product.Price < 0)
                {
                    throw new ValidationException("Product price cannot be negative");
                }

                var existingProduct = await GetByIdAsync(product.Id);
                
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;

                return await _productRepository.UpdateAsync(existingProduct);
            }
            catch (Exception ex) when (ex is not ValidationException && ex is not ProductNotFoundException)
            {
                _logger.LogError(ex, "Error updating product {@Product}", product);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                await GetByIdAsync(id); // Ensure product exists
                await _productRepository.DeleteAsync(id);
            }
            catch (Exception ex) when (ex is not ProductNotFoundException)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(Guid productId, int quantityChange)
        {
            try
            {
                var product = await GetByIdAsync(productId);
                
                if (product.StockQuantity + quantityChange < 0)
                {
                    return false;
                }

                product.StockQuantity += quantityChange;
                await _productRepository.UpdateAsync(product);
                return true;
            }
            catch (Exception ex) when (ex is not ProductNotFoundException)
            {
                _logger.LogError(ex, "Error updating stock for product {ProductId}", productId);
                throw;
            }
        }
    }
}