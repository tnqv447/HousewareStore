using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.Models;

namespace OrderApi.Data.Config {
    public class OrderConfig : IEntityTypeConfiguration<Order> {
        public void Configure (EntityTypeBuilder<Order> builder) {
            builder.HasKey (m => m.OrderId);
            builder.Property (m => m.OrderId).ValueGeneratedOnAdd ();

            builder.OwnsOne (o => o.Address, a => {
                a.Property (x => x.StreetAddress).HasColumnName ("StreetAddress");
                a.Property (x => x.Locality).HasColumnName ("Locality");
                a.Property (x => x.City).HasColumnName ("City");
                a.Property (x => x.Country).HasColumnName ("Country");
                a.Property (x => x.PostalCode).HasColumnName ("PostalCode");
            });

        }

    }
}