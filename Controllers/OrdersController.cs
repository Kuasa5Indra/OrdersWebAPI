using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersWebAPI.EfCore;
using OrdersWebAPI.Model;
using OrdersWebAPI.Request;
using OrdersWebAPI.Response;

namespace OrdersWebAPI.Controllers
{
    public class OrdersController : ApiControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public OrdersController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// Retrieve all orders
        /// </summary>
        /// <response code="200">OK</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders()
        {
            var orders =  await _appDbContext.Orders.ToListAsync();

            List<OrderResponse> ordersResponse = new();

            foreach (var order in orders)
            {
                ordersResponse.Add(new OrderResponse(order));
            }

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
            await _appDbContext.Orders.AddAsync(order);
            await _appDbContext.SaveChangesAsync();
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
            var order = await _appDbContext.Orders.FindAsync(id);

            if(order == null)
            {
                return NotFound();
            }

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
            var order = await _appDbContext.Orders.FindAsync(id);

            if (order == null)
            {
                return BadRequest();
            }

            order.CustomerName = request.CustomerName;
            order.TotalAmount = request.TotalAmount;

            await _appDbContext.SaveChangesAsync();

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
            var order = await _appDbContext.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            _appDbContext.Orders.Remove(order);

            await _appDbContext.SaveChangesAsync();

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
            var orderItems = await _appDbContext.OrderItems.Where(o => o.OrderId.Equals(orderId)).ToListAsync();

            List<OrderItemResponse> orderItemResponses = new();

            foreach(var item in orderItems)
            {
                orderItemResponses.Add(new OrderItemResponse(item));
            }

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

            await _appDbContext.OrderItems.AddAsync(orderItem);
            await _appDbContext.SaveChangesAsync();

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
            var orderItem = await _appDbContext.OrderItems.Where(o => o.OrderId.Equals(orderId))
                .Where(o => o.OrderItemId.Equals(id)).FirstOrDefaultAsync();

            if (orderItem == null)
            {
                return NotFound();
            }

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
            var orderItem = await _appDbContext.OrderItems.Where(o => o.OrderId.Equals(orderId))
                .Where(o => o.OrderItemId.Equals(id)).FirstOrDefaultAsync();

            if (orderItem == null)
            {
                return BadRequest();
            }

            orderItem.ProductName = request.ProductName;
            orderItem.UnitPrice = request.UnitPrice;
            orderItem.Quantity = request.Quantity;
            orderItem.TotalPrice = (request.UnitPrice * request.Quantity);

            await _appDbContext.SaveChangesAsync();

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
            var orderItem = await _appDbContext.OrderItems.Where(o => o.OrderId.Equals(orderId))
                .Where(o => o.OrderItemId.Equals(id)).FirstOrDefaultAsync();

            if (orderItem == null)
            {
                return NotFound();
            }

            _appDbContext.OrderItems.Remove(orderItem);
            await _appDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
