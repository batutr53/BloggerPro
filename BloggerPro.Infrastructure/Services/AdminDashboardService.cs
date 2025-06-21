using AutoMapper;
using BloggerPro.Application.DTOs.Dashboard;
using BloggerPro.Application.DTOs.Post;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminDashboardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DataResult<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            var users = await _unitOfWork.Users.Query().ToListAsync();
            var posts = await _unitOfWork.Posts.Query()
                .Include(p => p.Likes)
                .Include(p => p.Ratings)
                .ToListAsync();

            var comments = await _unitOfWork.Comments.Query().ToListAsync();

            var dto = new DashboardStatsDto
            {
                TotalUsers = users.Count,
                TotalPosts = posts.Count,
                TotalComments = comments.Count,
                TotalLikes = posts.Sum(p => p.Likes.Count),
                TotalRatings = posts.Sum(p => p.Ratings.Count),

                TopLikedPosts = _mapper.Map<List<PostListDto>>(
                    posts.OrderByDescending(p => p.Likes.Count).Take(5)),

                TopRatedPosts = _mapper.Map<List<PostListDto>>(
                    posts.Where(p => p.Ratings.Any())
                         .OrderByDescending(p => p.Ratings.Average(r => r.RatingValue))
                         .Take(5))
            };

            dto.MostActiveUsers = users
                .Select(u => new UserSummaryDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    TotalPosts = posts.Count(p => p.AuthorId == u.Id),
                    TotalComments = comments.Count(c => c.UserId == u.Id),
                    TotalRatings = posts.Sum(p => p.Ratings.Count(r => r.UserId == u.Id))
                })
                .OrderByDescending(u => u.TotalComments + u.TotalPosts + u.TotalRatings)
                .Take(5)
                .ToList();

            return new SuccessDataResult<DashboardStatsDto>(dto);
        }
    }

}
