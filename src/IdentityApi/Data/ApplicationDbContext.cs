using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityApi.Models;

namespace IdentityApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().OwnsOne(m => m.Address, a =>
            {
                a.Property(x => x.StreetAddress).HasColumnName("StreetAddress");
                a.Property(x => x.Locality).HasColumnName("Locality");
                a.Property(x => x.City).HasColumnName("City");
                a.Property(x => x.Country).HasColumnName("Country");
                a.Property(x => x.PostalCode).HasColumnName("PostalCode");
            });


            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
