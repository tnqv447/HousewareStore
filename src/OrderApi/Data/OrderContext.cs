using Microsoft.EntityFrameworkCore;
using OrderApi.Models;
using OrderApi.Data.Config;

namespace OrderApi.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Order>().OwnsOne(m => m.Address, a =>
            {
                a.Property(x => x.StreetAddress).HasColumnName("StreetAddress");
                a.Property(x => x.Locality).HasColumnName("Locality");
                a.Property(x => x.City).HasColumnName("City");
                a.Property(x => x.Country).HasColumnName("Country");
                a.Property(x => x.PostalCode).HasColumnName("PostalCode");
            });
            // builder.ApplyConfiguration(new OrderConfig());
            // builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}