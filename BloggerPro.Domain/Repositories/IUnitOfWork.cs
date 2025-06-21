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
    IGenericRepository<SeoMetadata> SeoMetadatas { get; }
    IGenericRepository<PostCategory> PostCategories { get; }
    IGenericRepository<PostTag> PostTags { get; }
    IGenericRepository<PostModule> PostModules { get; }
    IGenericRepository<Notification> Notifications { get; }
    IGenericRepository<UserFollower> UserFollowers { get; }


        
    Task<int> SaveChangesAsync();
}
