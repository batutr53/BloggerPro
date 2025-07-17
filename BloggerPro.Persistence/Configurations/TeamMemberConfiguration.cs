using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations;

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("TeamMembers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Position)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Bio)
            .HasMaxLength(1000);

        builder.Property(t => t.ImageUrl)
            .HasMaxLength(500);

        builder.Property(t => t.Email)
            .HasMaxLength(100);

        builder.Property(t => t.LinkedInUrl)
            .HasMaxLength(500);

        builder.Property(t => t.TwitterUrl)
            .HasMaxLength(500);

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        builder.Property(t => t.SortOrder)
            .HasDefaultValue(0);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);
    }
}