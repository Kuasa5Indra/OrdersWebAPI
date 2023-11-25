using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersWebAPI.EfCore;
using OrdersWebAPI.Model;
using OrdersWebAPI.Request;
using OrdersWebAPI.Response;

namespace OrdersWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public OrdersController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
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

        [HttpPost]
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

        [HttpGet("{id}")]
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

        [HttpPatch("{id}")]
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

        [HttpDelete("{id}")]
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

        [HttpGet("{orderId}/items")]
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

        [HttpPost("{orderId}/items")]
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

        [HttpGet("{orderId}/items/{id}")]
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

        [HttpPatch("{orderId}/items/{id}")]
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

        [HttpDelete("{orderId}/items/{id}")]
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
