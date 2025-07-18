using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations
{
    public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.UserId)
                .IsRequired();

            builder.Property(b => b.PostId)
                .IsRequired();

            builder.Property(b => b.BookmarkedAt)
                .IsRequired();

            builder.Property(b => b.Notes)
                .HasMaxLength(500);

            // Foreign key relationships
            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Post)
                .WithMany()
                .HasForeignKey(b => b.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: A user can only bookmark a post once
            builder.HasIndex(b => new { b.UserId, b.PostId })
                .IsUnique();

            builder.ToTable("Bookmarks");
        }
    }
}