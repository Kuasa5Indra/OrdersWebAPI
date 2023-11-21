using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersWebAPI.Model
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; }
        [Required, MaxLength(50)]
        public string? ProductName { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Range(1, int.MaxValue)]
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
