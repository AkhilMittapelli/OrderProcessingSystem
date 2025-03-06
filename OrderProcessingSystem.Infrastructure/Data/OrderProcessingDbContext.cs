using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Core.Entities;

namespace OrderProcessingSystem.Infrastructure.Data
{
    public class OrderProcessingDbContext : DbContext
    {
        public OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
        }
    }
}
