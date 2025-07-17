using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations;

public class FooterConfiguration : IEntityTypeConfiguration<Footer>
{
    public void Configure(EntityTypeBuilder<Footer> builder)
    {
        builder.ToTable("Footers");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.SectionTitle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(f => f.LinkUrl)
            .HasMaxLength(500);

        builder.Property(f => f.LinkText)
            .HasMaxLength(100);

        builder.Property(f => f.SortOrder)
            .HasDefaultValue(0);

        builder.Property(f => f.IsActive)
            .HasDefaultValue(true);

        builder.Property(f => f.FooterType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(f => f.IconClass)
            .HasMaxLength(100);

        builder.Property(f => f.ParentSection)
            .HasMaxLength(100);

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.UpdatedAt)
            .IsRequired(false);

        builder.Property(f => f.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(f => f.FooterType);
        builder.HasIndex(f => f.SortOrder);
        builder.HasIndex(f => f.IsActive);
    }
}