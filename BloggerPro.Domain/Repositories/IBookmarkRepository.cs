using BloggerPro.Domain.Entities;

namespace BloggerPro.Domain.Repositories
{
    public interface IBookmarkRepository : IGenericRepository<Bookmark>
    {
        Task<IQueryable<Bookmark>> GetUserBookmarksAsync(Guid userId);
        Task<bool> IsBookmarkedAsync(Guid userId, Guid postId);
        Task<Bookmark?> GetBookmarkAsync(Guid userId, Guid postId);
        Task<int> GetUserBookmarkCountAsync(Guid userId);
        Task<IQueryable<Bookmark>> GetUserBookmarksWithPostsAsync(Guid userId);
    }
}