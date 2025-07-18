using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations
{
    public class ReadingSessionConfiguration : IEntityTypeConfiguration<ReadingSession>
    {
        public void Configure(EntityTypeBuilder<ReadingSession> builder)
        {
            builder.ToTable("ReadingSessions");
            
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.ReadingTimeSeconds)
                .HasDefaultValue(0);
                
            builder.Property(x => x.ScrollPercentage)
                .HasDefaultValue(0);
                
            builder.Property(x => x.IsCompleted)
                .HasDefaultValue(false);
                
            builder.Property(x => x.PauseCount)
                .HasDefaultValue(0);
                
            builder.Property(x => x.ResumeCount)
                .HasDefaultValue(0);
                
            builder.Property(x => x.IpAddress)
                .HasMaxLength(45); // IPv6 support
                
            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);
                
            builder.Property(x => x.DeviceType)
                .HasMaxLength(50);
                
            builder.Property(x => x.ReferrerUrl)
                .HasMaxLength(1000);
            
            builder.Property(x => x.StartTime)
                .IsRequired();
                
            builder.Property(x => x.LastActivityTime)
                .IsRequired();
            
            // Relationships
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(x => x.Post)
                .WithMany()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.PostId);
            builder.HasIndex(x => x.StartTime);
            builder.HasIndex(x => x.IsCompleted);
            builder.HasIndex(x => new { x.UserId, x.PostId, x.StartTime });
            builder.HasIndex(x => new { x.UserId, x.IsCompleted });
        }
    }
}