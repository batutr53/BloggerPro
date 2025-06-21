using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Persistence.Contexts;

namespace BloggerPro.Persistence.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Post> Posts => new GenericRepository<Post>(_context);
    public IGenericRepository<User> Users => new GenericRepository<User>(_context);
    public IGenericRepository<Role> Roles => new GenericRepository<Role>(_context);
    public IGenericRepository<Category> Categories => new GenericRepository<Category>(_context);
    public IGenericRepository<Tag> Tags => new GenericRepository<Tag>(_context);
    public IGenericRepository<Comment> Comments => new GenericRepository<Comment>(_context);
    public IGenericRepository<CommentLike> CommentLikes => new GenericRepository<CommentLike>(_context);
    public IGenericRepository<PostRating> PostRatings =>  new GenericRepository<PostRating>(_context);
    public IGenericRepository<PostLike> PostLikes => new GenericRepository<PostLike>(_context);
    public IGenericRepository<SeoMetadata> SeoMetadatas => new GenericRepository<SeoMetadata>(_context);
    public IGenericRepository<PostCategory> PostCategories => new GenericRepository<PostCategory>(_context);
    public IGenericRepository<PostTag> PostTags => new GenericRepository<PostTag>(_context);
    public IGenericRepository<PostModule> PostModules => new GenericRepository<PostModule>(_context);
    public IGenericRepository<Notification> Notifications => new GenericRepository<Notification>(_context);
    public IGenericRepository<UserFollower> UserFollowers => new GenericRepository<UserFollower>(_context);


    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}