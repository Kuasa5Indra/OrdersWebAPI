using System;
using Microsoft.EntityFrameworkCore;
using OrdersWebAPI.Model;

namespace OrdersWebAPI.EfCore
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions options) : base(options)
		{
		}

		public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<OrderItem> OrderItems { get; set; }
	}
}

