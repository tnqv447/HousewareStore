using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ItemApi.Models;

namespace ItemApi.Data.Config
{
    public class ItemConfig : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd();

            builder.HasOne<Category>(m => m.Category)
                .WithMany(a => a.Items)
                .HasForeignKey(m => m.CategoryId);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(60)
                .HasAnnotation("MinLength", 3);

            builder.Property(m => m.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(m => m.Description)
                .HasMaxLength(200);
        }

    }
}