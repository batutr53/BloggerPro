﻿using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations;

public class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLike>
{
    public void Configure(EntityTypeBuilder<CommentLike> builder)
    {
        builder.HasKey(cl => new { cl.UserId, cl.CommentId }); 

        builder.HasOne(cl => cl.User)
            .WithMany(u => u.CommentLikes)
            .HasForeignKey(cl => cl.UserId);

        builder.HasOne(cl => cl.Comment)
            .WithMany(c => c.Likes)
            .HasForeignKey(cl => cl.CommentId);
    }
}
