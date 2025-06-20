using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations;

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.HasKey(pl => new { pl.PostId, pl.UserId });

        builder.HasOne(pl => pl.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(pl => pl.PostId);

        builder.HasOne(pl => pl.User)
            .WithMany(u => u.PostLikes)
            .HasForeignKey(pl => pl.UserId);
    }
}
