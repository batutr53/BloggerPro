using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations;

public class PostRatingConfiguration : IEntityTypeConfiguration<PostRating>
{
    public void Configure(EntityTypeBuilder<PostRating> builder)
    {
        builder.HasKey(x => new { x.UserId, x.PostId });

        builder.HasOne(x => x.Post)
               .WithMany(p => p.Ratings)
               .HasForeignKey(x => x.PostId);

        builder.HasOne(x => x.User)
               .WithMany(u => u.PostRatings)
               .HasForeignKey(x => x.UserId);

        builder.Property(x => x.RatingValue)
               .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired(false);
    }
}
