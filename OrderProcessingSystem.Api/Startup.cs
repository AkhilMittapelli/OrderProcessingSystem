using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderProcessingSystem.Core.Interfaces;
using OrderProcessingSystem.Core.Services;
using OrderProcessingSystem.Infrastructure.Data;
using OrderProcessingSystem.Infrastructure.Repositories;
using OrderProcessingSystem.Infrastructure.Services;

namespace OrderProcessingSystem.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Processing API", Version = "v1" });
            });

            // Register services
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderRepository, InMemoryOrderRepository>();
            services.AddScoped<IProductRepository, InMemoryProductRepository>();
            services.AddScoped<INotificationService, ConsoleNotificationService>();

            // Register background service
            services.AddHostedService<OrderFulfillmentBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Processing API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}