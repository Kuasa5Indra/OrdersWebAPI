using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OrdersWebAPI.EfCore
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required, StringLength(50)]
        public string? PersonName { get; set; }
    }
}
