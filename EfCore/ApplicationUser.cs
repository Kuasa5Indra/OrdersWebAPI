using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace OrdersWebAPI.EfCore
{
    [Index(nameof(NormalizedEmail), IsUnique = true)]
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required, StringLength(50)]
        public string? PersonName { get; set; }
        [Required]
        public override string? Email {  get; set; }
        [Required]
        public override string? UserName { get; set; }
        [Required, StringLength(15)]
        public override string? PhoneNumber { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
