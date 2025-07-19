using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(m => m.SentAt)
                .IsRequired();

            builder.Property(m => m.DeliveredAt)
                .IsRequired(false);

            builder.Property(m => m.ReadAt)
                .IsRequired(false);

            builder.Property(m => m.IsDeleted)
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(m => new { m.SenderId, m.ReceiverId, m.SentAt });
            builder.HasIndex(m => m.SentAt);
        }
    }
}