using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations
{
    public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
    {
        public void Configure(EntityTypeBuilder<UserActivity> builder)
        {
            builder.ToTable("UserActivities");
            
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.ActivityType)
                .HasMaxLength(50)
                .IsRequired();
                
            builder.Property(x => x.Description)
                .HasMaxLength(500)
                .IsRequired();
                
            builder.Property(x => x.ActivityData)
                .HasColumnType("text");
                
            builder.Property(x => x.IpAddress)
                .HasMaxLength(45); // IPv6 support
                
            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);
            
            builder.Property(x => x.ActivityDate)
                .IsRequired();
            
            // Relationships
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(x => x.Post)
                .WithMany()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.PostId);
            builder.HasIndex(x => x.ActivityType);
            builder.HasIndex(x => x.ActivityDate);
            builder.HasIndex(x => new { x.UserId, x.ActivityDate });
        }
    }
}