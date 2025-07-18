using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Persistence.Repositories
{
    public class BookmarkRepository : GenericRepository<Bookmark>, IBookmarkRepository
    {
        public BookmarkRepository(AppDbContext context) : base(context)
        {
        }

        public Task<IQueryable<Bookmark>> GetUserBookmarksAsync(Guid userId)
        {
            return Task.FromResult(_context.Bookmarks
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookmarkedAt)
                .AsQueryable());
        }

        public async Task<bool> IsBookmarkedAsync(Guid userId, Guid postId)
        {
            return await _context.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.PostId == postId);
        }

        public async Task<Bookmark?> GetBookmarkAsync(Guid userId, Guid postId)
        {
            return await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.PostId == postId);
        }

        public async Task<int> GetUserBookmarkCountAsync(Guid userId)
        {
            return await _context.Bookmarks
                .CountAsync(b => b.UserId == userId);
        }

        public Task<IQueryable<Bookmark>> GetUserBookmarksWithPostsAsync(Guid userId)
        {
            return Task.FromResult(_context.Bookmarks
                .Include(b => b.Post)
                    .ThenInclude(p => p.PostCategories)
                        .ThenInclude(pc => pc.Category)
                .Include(b => b.Post)
                    .ThenInclude(p => p.PostTags)
                        .ThenInclude(pt => pt.Tag)
                .Include(b => b.Post)
                    .ThenInclude(p => p.Author)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookmarkedAt)
                .AsQueryable());
        }
    }
}