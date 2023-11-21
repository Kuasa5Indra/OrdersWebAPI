using System.ComponentModel.DataAnnotations;

namespace OrdersWebAPI.Model
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        public string? OrderNumber { get; set; }
        [Required, MaxLength(50)]
        public string CustomerName { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Range(1, int.MaxValue)]
        public int TotalAmount { get; set; }

        // List Order Items
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
