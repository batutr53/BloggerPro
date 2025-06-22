using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Tablo adı
            builder.ToTable("Categories");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Zorunlu alanlar
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Slug)
                .IsRequired()
                .HasMaxLength(150);

            // Index (Slug benzersiz olsun)
            builder.HasIndex(c => c.Slug)
                .IsUnique();

            // Audit alanları (BaseEntity'den geldiğini varsayalım)
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.UpdatedAt).IsRequired(false);
        }
    }
}
