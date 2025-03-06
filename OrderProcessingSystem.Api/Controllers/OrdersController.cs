using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;
using OrderProcessingSystem.Core.Exceptions;

namespace OrderProcessingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                return StatusCode(500, "An error occurred while retrieving orders");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (OrderNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, "An error occurred while retrieving the order");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PlaceOrder([FromBody] Dictionary<Guid, int> productQuantities)
        {
            try
            {
                var order = await _orderService.PlaceOrderAsync(productQuantities);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InsufficientStockException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order");
                return StatusCode(500, "An error occurred while placing the order");
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelOrder(Guid id)
        {
            try
            {
                await _orderService.CancelOrderAsync(id);
                return Ok();
            }
            catch (OrderNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return StatusCode(500, "An error occurred while cancelling the order");
            }
        }

        [HttpPost("{id}/fulfill")]
        public async Task<ActionResult> FulfillOrder(Guid id)
        {
            try
            {
                await _orderService.FulfillOrderAsync(id);
                return Ok();
            }
            catch (OrderNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fulfilling order {OrderId}", id);
                return StatusCode(500, "An error occurred while fulfilling the order");
            }
        }
    }
}
