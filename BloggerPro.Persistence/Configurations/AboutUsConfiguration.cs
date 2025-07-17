using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations;

public class AboutUsConfiguration : IEntityTypeConfiguration<AboutUs>
{
    public void Configure(EntityTypeBuilder<AboutUs> builder)
    {
        builder.ToTable("AboutUs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Content)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(a => a.Mission)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.Vision)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true);

        builder.Property(a => a.SortOrder)
            .HasDefaultValue(0);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        builder.Property(a => a.IsDeleted)
            .HasDefaultValue(false);
    }
}