using Microsoft.EntityFrameworkCore;
using BloggerPro.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BloggerPro.Persistence.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>(); 
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PostCategory> PostCategories => Set<PostCategory>();
    public DbSet<PostTag> PostTags => Set<PostTag>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<PostRating> PostRatings => Set<PostRating>();
    public DbSet<CommentLike> CommentLikes => Set<CommentLike>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
