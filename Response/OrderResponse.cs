using OrdersWebAPI.Model;

namespace OrdersWebAPI.Response
{
    public class OrderResponse
    {
        public Guid OrderId { get; }
        public string? OrderNumber { get; }
        public string? CustomerName { get; }
        public DateTime OrderDate { get; }
        public int TotalAmount { get; }

        public OrderResponse(Order order)
        {
            OrderId = order.OrderId;
            OrderNumber = order.OrderNumber;
            CustomerName = order.CustomerName;
            OrderDate = order.OrderDate;
            TotalAmount = order.TotalAmount;
        }
    }
}
