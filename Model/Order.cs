using System.ComponentModel.DataAnnotations;

namespace OrdersWebAPI.Model
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        public string? OrderNumber { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
    }
}
