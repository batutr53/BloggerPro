using AutoMapper;
using BloggerPro.Application.DTOs.Bookmark;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookmarkService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult> BookmarkPostAsync(BookmarkCreateDto bookmarkCreateDto)
        {
            try
            {
                // Check if bookmark already exists
                var existingBookmark = await _unitOfWork.Bookmarks.GetBookmarkAsync(bookmarkCreateDto.UserId, bookmarkCreateDto.PostId);
                if (existingBookmark != null)
                {
                    return new ErrorResult("Bu yazı zaten favorilere eklenmiş.", 400);
                }

                // Check if post exists
                var post = await _unitOfWork.Posts.GetByIdAsync(bookmarkCreateDto.PostId);
                if (post == null)
                {
                    return new ErrorResult("Yazı bulunamadı.", 404);
                }

                // Create bookmark
                var bookmark = new Bookmark
                {
                    UserId = bookmarkCreateDto.UserId,
                    PostId = bookmarkCreateDto.PostId,
                    BookmarkedAt = DateTime.UtcNow,
                    Notes = bookmarkCreateDto.Notes
                };

                await _unitOfWork.Bookmarks.AddAsync(bookmark);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessResult("Yazı favorilere eklendi.", 200);
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Favori eklenirken bir hata oluştu: {ex.Message}", 500);
            }
        }

        public async Task<IResult> RemoveBookmarkAsync(Guid userId, Guid postId)
        {
            try
            {
                var bookmark = await _unitOfWork.Bookmarks.GetBookmarkAsync(userId, postId);
                if (bookmark == null)
                {
                    return new ErrorResult("Favori bulunamadı.", 404);
                }

                await _unitOfWork.Bookmarks.DeleteAsync(bookmark);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessResult("Yazı favorilerden kaldırıldı.", 200);
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Favori kaldırılırken bir hata oluştu: {ex.Message}", 500);
            }
        }

        public async Task<IDataResult<bool>> IsBookmarkedAsync(Guid userId, Guid postId)
        {
            try
            {
                var isBookmarked = await _unitOfWork.Bookmarks.IsBookmarkedAsync(userId, postId);
                return new SuccessDataResult<bool>(isBookmarked, "Favori durumu alındı.", 200);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<bool>(false, $"Favori durumu alınırken bir hata oluştu: {ex.Message}", 500);
            }
        }

        public async Task<IDataResult<List<BookmarkListDto>>> GetUserBookmarksAsync(Guid userId)
        {
            try
            {
                var bookmarksQuery = await _unitOfWork.Bookmarks.GetUserBookmarksWithPostsAsync(userId);
                var bookmarks = await bookmarksQuery.ToListAsync();

                var bookmarkDtos = _mapper.Map<List<BookmarkListDto>>(bookmarks);
                return new SuccessDataResult<List<BookmarkListDto>>(bookmarkDtos, "Favoriler getirildi.", 200);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<BookmarkListDto>>(new List<BookmarkListDto>(), $"Favoriler getirilirken bir hata oluştu: {ex.Message}", 500);
            }
        }

        public async Task<IDataResult<int>> GetUserBookmarkCountAsync(Guid userId)
        {
            try
            {
                var count = await _unitOfWork.Bookmarks.GetUserBookmarkCountAsync(userId);
                return new SuccessDataResult<int>(count, "Favori sayısı getirildi.", 200);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<int>(0, $"Favori sayısı getirilirken bir hata oluştu: {ex.Message}", 500);
            }
        }

        public async Task<IDataResult<BookmarkDetailDto>> GetBookmarkAsync(Guid userId, Guid postId)
        {
            try
            {
                var bookmarksQuery = await _unitOfWork.Bookmarks.GetUserBookmarksWithPostsAsync(userId);
                var bookmark = await bookmarksQuery.FirstOrDefaultAsync(b => b.PostId == postId);

                if (bookmark == null)
                {
                    return new ErrorDataResult<BookmarkDetailDto>(null, "Favori bulunamadı.", 404);
                }

                var bookmarkDto = _mapper.Map<BookmarkDetailDto>(bookmark);
                return new SuccessDataResult<BookmarkDetailDto>(bookmarkDto, "Favori getirildi.", 200);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<BookmarkDetailDto>(null, $"Favori getirilirken bir hata oluştu: {ex.Message}", 500);
            }
        }
    }
}