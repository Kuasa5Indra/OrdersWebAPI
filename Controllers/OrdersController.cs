using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersWebAPI.EfCore;
using OrdersWebAPI.Model;
using OrdersWebAPI.Request;
using OrdersWebAPI.Response;

namespace OrdersWebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(AppDbContext appDbContext, ILogger<OrdersController> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all orders
        /// </summary>
        /// <response code="200">OK</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders()
        {
            _logger.LogInformation("Getting order data");
            var orders =  await _appDbContext.Orders.ToListAsync();

            List<OrderResponse> ordersResponse = new();

            foreach (var order in orders)
            {
                ordersResponse.Add(new OrderResponse(order));
            }
            _logger.LogInformation("Successfully get order data");

            return Ok(ordersResponse);
        }

        /// <summary>
        /// Add a new order
        /// </summary>
        /// <response code="201">Created</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateOrder(OrderAddRequest request)
        {
            var datetime = DateTime.Now.ToString("yyyyMMddHHmm");
            Order order = new()
            {
                OrderNumber = String.Concat("Order_", datetime),
                CustomerName = request.CustomerName,
                OrderDate = request.OrderDate,
                TotalAmount = request.TotalAmount
            };
            _logger.LogInformation("Attempt to create order");
            await _appDbContext.Orders.AddAsync(order);
            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully create order");
            OrderResponse response = new(order);
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, response); 
        }

        /// <summary>
        /// Retrieve an order by ID
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="404">if order with the given ID does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            _logger.LogInformation("Attempt to find order {id} ", id);
            var order = await _appDbContext.Orders.FindAsync(id);

            if(order == null)
            {
                _logger.LogError("Order {id} not found", id);
                return Problem(statusCode: 404, detail: "Order doesn't exist");
            }

            _logger.LogInformation("Successfully get order {id}", id);
            OrderResponse response = new(order);

            return Ok(response);
        }

        /// <summary>
        /// Update an existing order
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">if the ID in the request body does not match the route parameter</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrder(Guid id, OrderUpdateRequest request)
        {
            _logger.LogInformation("Attempt to find order {id} ", id);
            var order = await _appDbContext.Orders.FindAsync(id);

            if (order == null)
            {
                _logger.LogError("Order {id} not found", id);
                return Problem(statusCode: 400, detail: "Order doesn't exist while updating data");
            }

            _logger.LogInformation("Attempt to update Order");
            order.CustomerName = request.CustomerName;
            order.TotalAmount = request.TotalAmount;

            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully update Order");

            OrderResponse response = new(order);

            return Ok(response);
        }

        /// <summary>
        /// Delete an order
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="404">if order with the given ID does not exist</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            _logger.LogInformation("Attempt to find order {id} ", id);
            var order = await _appDbContext.Orders.FindAsync(id);

            if (order == null)
            {
                _logger.LogError("Order {id} not found", id);
                return Problem(statusCode: 404, detail: "Order doesn't exist");
            }

            _logger.LogInformation("Attempt to delete Order");
            _appDbContext.Orders.Remove(order);

            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully delete Order");

            return NoContent();
        }

        /// <summary>
        /// Retrieve all order items for a specific order
        /// </summary>
        /// <response code="200">OK</response>
        [HttpGet("{orderId}/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrderItems(Guid orderId)
        {
            _logger.LogInformation("Getting order items based on order id {id}", orderId);
            var orderItems = await _appDbContext.OrderItems.Where(o => o.OrderId.Equals(orderId)).ToListAsync();

            List<OrderItemResponse> orderItemResponses = new();

            foreach(var item in orderItems)
            {
                orderItemResponses.Add(new OrderItemResponse(item));
            }
            _logger.LogInformation("Successfully get order items based on order id {id}", orderId);

            return Ok(orderItemResponses);
        }

        /// <summary>
        /// Add a new order item
        /// </summary>
        /// <response code="201">Created</response>
        [HttpPost("{orderId}/items")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateOrderItem(Guid orderId, OrderItemAddRequest request)
        {
            OrderItem orderItem = new()
            {
                OrderId = orderId,
                ProductName = request.ProductName,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                TotalPrice = (request.Quantity * request.UnitPrice)
            };

            _logger.LogInformation("Create order item based on order id {id}", orderId);
            await _appDbContext.OrderItems.AddAsync(orderItem);
            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully create order item based on order id {id}", orderId);

            OrderItemResponse response = new(orderItem);

            return CreatedAtAction(nameof(GetOrderItem), new { orderId = orderItem.OrderId, id = orderItem.OrderItemId}, orderItem);
        }

        /// <summary>
        /// Retrieve an order item by ID
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="404">if order item with the given ID does not exist</response>
        [HttpGet("{orderId}/items/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderItem(Guid orderId, Guid id)
        {
            _logger.LogInformation("Attempt to find order item {id} based on order {orderId}", id, orderId);
            var orderItem = await _appDbContext.OrderItems.Where(o => o.OrderId.Equals(orderId))
                .Where(o => o.OrderItemId.Equals(id)).FirstOrDefaultAsync();

            if (orderItem == null)
            {
                _logger.LogError("Order item {id} not found", id);
                return Problem(statusCode: 404, detail: "Order item doesn't exist");
            }

            _logger.LogInformation("Successfully get order item {id} based on order {orderId}", id, orderId);
            OrderItemResponse response = new(orderItem);

            return Ok(response);
        }

        /// <summary>
        /// Update an existing order item
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">if the ID in the request body does not match the route parameter</response>
        [HttpPatch("{orderId}/items/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderItem(Guid orderId, Guid id, OrderItemUpdateRequest request)
        {
            _logger.LogInformation("Attempt to find order item {id} based on order {orderId}", id, orderId);
            var orderItem = await _appDbContext.OrderItems.Where(o => o.OrderId.Equals(orderId))
                .Where(o => o.OrderItemId.Equals(id)).FirstOrDefaultAsync();

            if (orderItem == null)
            {
                _logger.LogError("Order item {id} not found", id);
                return Problem(statusCode: 400, detail: "Order item doesn't exist while updating data");
            }

            _logger.LogInformation("Attempt to update order item");
            orderItem.ProductName = request.ProductName;
            orderItem.UnitPrice = request.UnitPrice;
            orderItem.Quantity = request.Quantity;
            orderItem.TotalPrice = (request.UnitPrice * request.Quantity);

            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully update order item");

            OrderItemResponse response = new(orderItem);

            return Ok(response);
        }

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="404">if order item with the given ID does not exist</response>
        [HttpDelete("{orderId}/items/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrderItem(Guid orderId, Guid id)
        {
            _logger.LogInformation("Attempt to find order item {id} based on order {orderId}", id, orderId);
            var orderItem = await _appDbContext.OrderItems.Where(o => o.OrderId.Equals(orderId))
                .Where(o => o.OrderItemId.Equals(id)).FirstOrDefaultAsync();

            if (orderItem == null)
            {
                _logger.LogError("Order item {id} not found", id);
                return Problem(statusCode: 404, detail: "Order item doesn't exist");
            }

            _logger.LogInformation("Attempt to delete order item");
            _appDbContext.OrderItems.Remove(orderItem);
            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully delete order item");

            return NoContent();
        }
    }
}
