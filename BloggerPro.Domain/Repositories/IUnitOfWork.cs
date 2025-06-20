using BloggerPro.Domain.Entities;

namespace BloggerPro.Domain.Repositories;

public interface IUnitOfWork
{
    IGenericRepository<Post> Posts { get; }
    IGenericRepository<User> Users { get; }
    IGenericRepository<Role> Roles { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<Tag> Tags { get; }
    IGenericRepository<Comment> Comments { get; }
    IGenericRepository<CommentLike> CommentLikes { get; }
    IGenericRepository<PostRating> PostRatings { get; }
    IGenericRepository<PostLike> PostLikes { get; }

    Task<int> SaveChangesAsync();
}
