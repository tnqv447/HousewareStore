using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ItemApi.Models;
using ItemApi.Data.Config;

namespace ItemApi.Data
{
    public class ItemContext : DbContext
    {
        public ItemContext(DbContextOptions<ItemContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ItemConfig());
            builder.ApplyConfiguration(new CategoryConfig());
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}