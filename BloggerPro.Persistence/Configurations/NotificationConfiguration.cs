using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);
            
            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(n => n.RelatedEntityType)
                .HasMaxLength(50);
                
            builder.Property(n => n.CreatedAt)
                .IsRequired();
                
            // Relationship with User
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
