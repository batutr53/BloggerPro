using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations
{
    public class UserPresenceConfiguration : IEntityTypeConfiguration<UserPresence>
    {
        public void Configure(EntityTypeBuilder<UserPresence> builder)
        {
            builder.ToTable("UserPresences");

            builder.HasKey(up => up.UserId);

            builder.Property(up => up.IsOnline)
                .HasDefaultValue(false);

            builder.Property(up => up.LastSeen)
                .IsRequired();

            builder.Property(up => up.ConnectionId)
                .HasMaxLength(100);

            builder.Property(up => up.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(up => up.User)
                .WithOne()
                .HasForeignKey<UserPresence>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(up => up.IsOnline);
            builder.HasIndex(up => up.LastSeen);
        }
    }
}