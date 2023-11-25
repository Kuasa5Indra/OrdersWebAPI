using OrdersWebAPI.Model;
namespace OrdersWebAPI.Response
{
    public class OrderItemResponse
    {
        public Guid OrderItemId { get; }
        public string? ProductName { get; }
        public int Quantity { get; }
        public int UnitPrice { get; }
        public int TotalPrice { get; }
        public Guid OrderId { get; }

        public OrderItemResponse(OrderItem orderItem)
        {
            OrderItemId = orderItem.OrderItemId;
            ProductName = orderItem.ProductName;
            Quantity = orderItem.Quantity;
            UnitPrice = orderItem.UnitPrice;
            TotalPrice = orderItem.TotalPrice;
            OrderId = orderItem.OrderId;
        }
    }
}
