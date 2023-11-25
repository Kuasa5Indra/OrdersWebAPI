using System.ComponentModel.DataAnnotations;

namespace OrdersWebAPI.Request
{
    public class OrderAddRequest
    {
        [Required, MaxLength(50)]
        public string? CustomerName { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Range(1, int.MaxValue)]
        public int TotalAmount { get; set; }
    }
}
