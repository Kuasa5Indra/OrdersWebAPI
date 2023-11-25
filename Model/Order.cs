using System.ComponentModel.DataAnnotations;

namespace OrdersWebAPI.Model
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();
        [Required, MaxLength(20)]
        public string? OrderNumber { get; set; }
        [Required, MaxLength(50)]
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
        // List Order Items
        public ICollection<OrderItem>? OrderItems { get; set; } 
    }
}
