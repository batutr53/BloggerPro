using AutoMapper;
using BloggerPro.Application.DTOs.UserDashboard;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloggerPro.Infrastructure.Services
{
    public class UserDashboardService : IUserDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserDashboardService> _logger;

        public UserDashboardService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserDashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IDataResult<UserDashboardStatsDto>> GetUserDashboardStatsAsync(Guid userId)
        {
            try
            {
                var now = DateTime.UtcNow;
                var monthStart = new DateTime(now.Year, now.Month, 1);
                var weekStart = now.AddDays(-(int)now.DayOfWeek);

                // Get reading sessions
                var readingSessions = await _unitOfWork.ReadingSessions
                    .Query()
                    .Where(rs => rs.UserId == userId && rs.EndTime != null)
                    .ToListAsync();

                // Get bookmarks count
                var bookmarksCount = await _unitOfWork.Bookmarks
                    .CountAsync(b => b.UserId == userId);

                // Get activities
                var activities = await _unitOfWork.UserActivities
                    .Query()
                    .Where(a => a.UserId == userId)
                    .ToListAsync();

                // Get user comments and likes
                var commentsCount = await _unitOfWork.Comments
                    .CountAsync(c => c.UserId == userId);

                var likesCount = await _unitOfWork.PostLikes
                    .CountAsync(pl => pl.UserId == userId);

                // Calculate stats
                var totalReadPosts = readingSessions.Select(rs => rs.PostId).Distinct().Count();
                var totalReadingTime = readingSessions.Sum(rs => rs.ReadingTimeMinutes);
                var activeDays = readingSessions.Select(rs => rs.StartTime.Date).Distinct().Count();
                var postsReadThisMonth = readingSessions.Count(rs => rs.StartTime >= monthStart);
                var postsReadThisWeek = readingSessions.Count(rs => rs.StartTime >= weekStart);
                var averageReadingTime = totalReadPosts > 0 ? (double)totalReadingTime / totalReadPosts : 0;

                // Get favorite category
                var favoriteCategory = await GetFavoriteCategoryAsync(userId);

                // Get last activity
                var lastActivity = activities.OrderByDescending(a => a.ActivityDate).FirstOrDefault();

                // Calculate consecutive active days
                var consecutiveDays = await CalculateConsecutiveActiveDaysAsync(userId);

                var stats = new UserDashboardStatsDto
                {
                    TotalReadPosts = totalReadPosts,
                    TotalBookmarks = bookmarksCount,
                    TotalReadingTime = totalReadingTime,
                    TotalActiveDays = activeDays,
                    PostsReadThisMonth = postsReadThisMonth,
                    PostsReadThisWeek = postsReadThisWeek,
                    AverageReadingTime = Math.Round(averageReadingTime, 1),
                    TotalCommentsLeft = commentsCount,
                    TotalLikesGiven = likesCount,
                    FavoriteCategory = favoriteCategory,
                    LastActivityDate = lastActivity?.ActivityDate ?? DateTime.MinValue,
                    ConsecutiveActiveDays = consecutiveDays
                };

                return new SuccessDataResult<UserDashboardStatsDto>(stats, "Dashboard statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard stats for user {UserId}", userId);
                return new ErrorDataResult<UserDashboardStatsDto>("Error retrieving dashboard statistics");
            }
        }

        public async Task<IDataResult<List<UserActivityDto>>> GetUserActivitiesAsync(Guid userId, int take = 10)
        {
            try
            {
                var activities = await _unitOfWork.UserActivities
                    .Query()
                    .Include(a => a.Post)
                    .ThenInclude(p => p.PostCategories)
                    .ThenInclude(pc => pc.Category)
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.ActivityDate)
                    .Take(take)
                    .ToListAsync();

                var activityDtos = activities.Select(a => new UserActivityDto
                {
                    Id = a.Id,
                    ActivityType = a.ActivityType,
                    Title = GetActivityTitle(a),
                    Description = a.Description,
                    ActivityDate = a.ActivityDate,
                    PostSlug = a.Post?.Slug ?? string.Empty,
                    PostTitle = a.Post?.Title ?? string.Empty,
                    PostImage = a.Post?.FeaturedImage ?? string.Empty,
                    CategoryName = a.Post?.PostCategories?.FirstOrDefault()?.Category?.Name ?? string.Empty,
                    Icon = GetActivityIcon(a.ActivityType),
                    Color = GetActivityColor(a.ActivityType)
                }).ToList();

                return new SuccessDataResult<List<UserActivityDto>>(activityDtos, "User activities retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user activities for user {UserId}", userId);
                return new ErrorDataResult<List<UserActivityDto>>("Error retrieving user activities");
            }
        }

        public async Task<IDataResult<List<RecentPostDto>>> GetRecentPostsAsync(Guid userId, int take = 10)
        {
            try
            {
                var recentSessions = await _unitOfWork.ReadingSessions
                    .Query()
                    .Include(rs => rs.Post)
                    .ThenInclude(p => p.PostCategories)
                    .ThenInclude(pc => pc.Category)
                    .Include(rs => rs.Post)
                    .ThenInclude(p => p.Author)
                    .Include(rs => rs.Post)
                    .ThenInclude(p => p.Comments)
                    .Where(rs => rs.UserId == userId)
                    .OrderByDescending(rs => rs.StartTime)
                    .Take(take)
                    .ToListAsync();

                var postDtos = new List<RecentPostDto>();
                foreach (var session in recentSessions)
                {
                    if (session.Post == null) continue;

                    var isBookmarked = await _unitOfWork.Bookmarks
                        .AnyAsync(b => b.UserId == userId && b.PostId == session.PostId);

                    var isLiked = await _unitOfWork.PostLikes
                        .AnyAsync(pl => pl.UserId == userId && pl.PostId == session.PostId);

                    var totalReadingTime = await _unitOfWork.ReadingSessions
                        .Query()
                        .Where(rs => rs.UserId == userId && rs.PostId == session.PostId)
                        .SumAsync(rs => rs.ReadingTimeMinutes);

                    var visitCount = await _unitOfWork.ReadingSessions
                        .CountAsync(rs => rs.UserId == userId && rs.PostId == session.PostId);

                    postDtos.Add(new RecentPostDto
                    {
                        Id = session.Post.Id,
                        Title = session.Post.Title,
                        Slug = session.Post.Slug,
                        Excerpt = session.Post.Excerpt,
                        FeaturedImage = session.Post.FeaturedImage,
                        CategoryName = session.Post.PostCategories?.FirstOrDefault()?.Category?.Name ?? string.Empty,
                        AuthorName = session.Post.Author?.UserName ?? string.Empty,
                        VisitedAt = session.StartTime,
                        ReadingTimeMinutes = session.ReadingTimeMinutes,
                        IsBookmarked = isBookmarked,
                        IsLiked = isLiked,
                        TotalReadingTime = totalReadingTime,
                        VisitCount = visitCount,
                        CreatedAt = session.Post.CreatedAt,
                        ViewCount = session.Post.ViewCount,
                        LikeCount = session.Post.LikeCount,
                        CommentCount = session.Post.Comments?.Count ?? 0
                    });
                }

                return new SuccessDataResult<List<RecentPostDto>>(postDtos, "Recent posts retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent posts for user {UserId}", userId);
                return new ErrorDataResult<List<RecentPostDto>>("Error retrieving recent posts");
            }
        }

        public async Task<IDataResult<List<ReadingSessionDto>>> GetActiveReadingSessionsAsync(Guid userId)
        {
            try
            {
                var activeSessions = await _unitOfWork.ReadingSessions
                    .Query()
                    .Include(rs => rs.Post)
                    .ThenInclude(p => p.PostCategories)
                    .ThenInclude(pc => pc.Category)
                    .Where(rs => rs.UserId == userId && rs.EndTime == null)
                    .Where(rs => DateTime.UtcNow.Subtract(rs.LastActivityTime).TotalMinutes < 30)
                    .OrderByDescending(rs => rs.LastActivityTime)
                    .ToListAsync();

                var sessionDtos = activeSessions.Select(rs => new ReadingSessionDto
                {
                    Id = rs.Id,
                    PostId = rs.PostId,
                    PostTitle = rs.Post?.Title ?? string.Empty,
                    PostSlug = rs.Post?.Slug ?? string.Empty,
                    PostImage = rs.Post?.FeaturedImage ?? string.Empty,
                    CategoryName = rs.Post?.PostCategories?.FirstOrDefault()?.Category?.Name ?? string.Empty,
                    StartTime = rs.StartTime,
                    EndTime = rs.EndTime,
                    ReadingTimeSeconds = rs.ReadingTimeSeconds,
                    IsCompleted = rs.IsCompleted,
                    ScrollPercentage = rs.ScrollPercentage,
                    LastActivityTime = rs.LastActivityTime
                }).ToList();

                return new SuccessDataResult<List<ReadingSessionDto>>(sessionDtos, "Active reading sessions retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active reading sessions for user {UserId}", userId);
                return new ErrorDataResult<List<ReadingSessionDto>>("Error retrieving active reading sessions");
            }
        }

        public async Task<IResult> TrackPostViewAsync(Guid userId, Guid postId, string ipAddress, string userAgent)
        {
            try
            {
                var activity = new UserActivity
                {
                    UserId = userId,
                    PostId = postId,
                    ActivityType = "PostView",
                    Description = "Viewed a post",
                    ActivityDate = DateTime.UtcNow,
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                await _unitOfWork.UserActivities.AddAsync(activity);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessResult("Post view tracked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking post view for user {UserId}, post {PostId}", userId, postId);
                return new ErrorResult("Error tracking post view");
            }
        }

        public async Task<IDataResult<ReadingSessionDto>> StartReadingSessionAsync(Guid userId, Guid postId, string ipAddress, string userAgent, string deviceType, string referrerUrl = "")
        {
            try
            {
                // Check if there's already an active session for this post
                var existingSession = await _unitOfWork.ReadingSessions
                    .FirstOrDefaultAsync(rs => rs.UserId == userId && rs.PostId == postId && rs.EndTime == null);

                if (existingSession != null)
                {
                    // Update existing session
                    existingSession.LastActivityTime = DateTime.UtcNow;
                    existingSession.ResumeCount++;
                    _unitOfWork.ReadingSessions.Update(existingSession);
                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessDataResult<ReadingSessionDto>(_mapper.Map<ReadingSessionDto>(existingSession), "Reading session resumed");
                }

                // Create new session
                var session = new ReadingSession
                {
                    UserId = userId,
                    PostId = postId,
                    StartTime = DateTime.UtcNow,
                    LastActivityTime = DateTime.UtcNow,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    DeviceType = deviceType,
                    ReferrerUrl = referrerUrl
                };

                await _unitOfWork.ReadingSessions.AddAsync(session);
                await _unitOfWork.SaveChangesAsync();

                var sessionDto = _mapper.Map<ReadingSessionDto>(session);
                return new SuccessDataResult<ReadingSessionDto>(sessionDto, "Reading session started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting reading session for user {UserId}, post {PostId}", userId, postId);
                return new ErrorDataResult<ReadingSessionDto>("Error starting reading session");
            }
        }

        public async Task<IResult> UpdateReadingSessionAsync(Guid userId, Guid postId, int readingTimeSeconds, int scrollPercentage)
        {
            try
            {
                var session = await _unitOfWork.ReadingSessions
                    .FirstOrDefaultAsync(rs => rs.UserId == userId && rs.PostId == postId && rs.EndTime == null);

                if (session == null)
                {
                    return new ErrorResult("Active reading session not found");
                }

                session.ReadingTimeSeconds = readingTimeSeconds;
                session.ScrollPercentage = scrollPercentage;
                session.LastActivityTime = DateTime.UtcNow;

                _unitOfWork.ReadingSessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessResult("Reading session updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reading session for user {UserId}, post {PostId}", userId, postId);
                return new ErrorResult("Error updating reading session");
            }
        }

        public async Task<IResult> CompleteReadingSessionAsync(Guid userId, Guid postId)
        {
            try
            {
                var session = await _unitOfWork.ReadingSessions
                    .FirstOrDefaultAsync(rs => rs.UserId == userId && rs.PostId == postId && rs.EndTime == null);

                if (session == null)
                {
                    return new ErrorResult("Active reading session not found");
                }

                session.EndTime = DateTime.UtcNow;
                session.IsCompleted = true;
                session.LastActivityTime = DateTime.UtcNow;

                _unitOfWork.ReadingSessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessResult("Reading session completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing reading session for user {UserId}, post {PostId}", userId, postId);
                return new ErrorResult("Error completing reading session");
            }
        }

        public async Task<IDataResult<Dictionary<string, int>>> GetReadingStatsAsync(Guid userId)
        {
            try
            {
                var now = DateTime.UtcNow;
                var stats = new Dictionary<string, int>();

                // Last 7 days
                for (int i = 6; i >= 0; i--)
                {
                    var date = now.AddDays(-i).Date;
                    var count = await _unitOfWork.ReadingSessions
                        .CountAsync(rs => rs.UserId == userId && rs.StartTime.Date == date);
                    stats[date.ToString("MMM dd")] = count;
                }

                return new SuccessDataResult<Dictionary<string, int>>(stats, "Reading stats retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reading stats for user {UserId}", userId);
                return new ErrorDataResult<Dictionary<string, int>>("Error retrieving reading stats");
            }
        }

        public async Task<IDataResult<Dictionary<string, int>>> GetMonthlyReadingStatsAsync(Guid userId)
        {
            try
            {
                var now = DateTime.UtcNow;
                var stats = new Dictionary<string, int>();

                // Last 12 months
                for (int i = 11; i >= 0; i--)
                {
                    var date = now.AddMonths(-i);
                    var monthStart = new DateTime(date.Year, date.Month, 1);
                    var monthEnd = monthStart.AddMonths(1);
                    
                    var count = await _unitOfWork.ReadingSessions
                        .CountAsync(rs => rs.UserId == userId && rs.StartTime >= monthStart && rs.StartTime < monthEnd);
                    stats[date.ToString("MMM yyyy")] = count;
                }

                return new SuccessDataResult<Dictionary<string, int>>(stats, "Monthly reading stats retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly reading stats for user {UserId}", userId);
                return new ErrorDataResult<Dictionary<string, int>>("Error retrieving monthly reading stats");
            }
        }

        public async Task<IDataResult<List<string>>> GetFavoriteCategoriesAsync(Guid userId, int take = 5)
        {
            try
            {
                var categories = await _unitOfWork.ReadingSessions
                    .Query()
                    .Include(rs => rs.Post)
                    .ThenInclude(p => p.PostCategories)
                    .ThenInclude(pc => pc.Category)
                    .Where(rs => rs.UserId == userId)
                    .SelectMany(rs => rs.Post.PostCategories.Select(pc => pc.Category.Name))
                    .GroupBy(name => name)
                    .OrderByDescending(g => g.Count())
                    .Take(take)
                    .Select(g => g.Key)
                    .ToListAsync();

                return new SuccessDataResult<List<string>>(categories, "Favorite categories retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving favorite categories for user {UserId}", userId);
                return new ErrorDataResult<List<string>>("Error retrieving favorite categories");
            }
        }

        private async Task<string> GetFavoriteCategoryAsync(Guid userId)
        {
            try
            {
                var favoriteCategory = await _unitOfWork.ReadingSessions
                    .Query()
                    .Include(rs => rs.Post)
                    .ThenInclude(p => p.PostCategories)
                    .ThenInclude(pc => pc.Category)
                    .Where(rs => rs.UserId == userId)
                    .SelectMany(rs => rs.Post.PostCategories.Select(pc => pc.Category.Name))
                    .GroupBy(name => name)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefaultAsync();

                return favoriteCategory?.Key ?? "Genel";
            }
            catch
            {
                return "Genel";
            }
        }

        private async Task<int> CalculateConsecutiveActiveDaysAsync(Guid userId)
        {
            try
            {
                var activities = await _unitOfWork.UserActivities
                    .Query()
                    .Where(a => a.UserId == userId)
                    .Select(a => a.ActivityDate.Date)
                    .Distinct()
                    .OrderByDescending(date => date)
                    .ToListAsync();

                if (!activities.Any()) return 0;

                var consecutiveDays = 0;
                var currentDate = DateTime.UtcNow.Date;

                foreach (var activityDate in activities)
                {
                    if (activityDate == currentDate || activityDate == currentDate.AddDays(-consecutiveDays))
                    {
                        consecutiveDays++;
                        currentDate = activityDate;
                    }
                    else
                    {
                        break;
                    }
                }

                return consecutiveDays;
            }
            catch
            {
                return 0;
            }
        }

        private static string GetActivityTitle(UserActivity activity)
        {
            return activity.ActivityType switch
            {
                "PostView" => "Yazı Okundu",
                "Bookmark" => "Favorilere Eklendi",
                "Comment" => "Yorum Yapıldı",
                "Like" => "Beğenildi",
                "Share" => "Paylaşıldı",
                _ => "Aktivite"
            };
        }

        private static string GetActivityIcon(string activityType)
        {
            return activityType switch
            {
                "PostView" => "fas fa-eye",
                "Bookmark" => "fas fa-bookmark",
                "Comment" => "fas fa-comment",
                "Like" => "fas fa-heart",
                "Share" => "fas fa-share",
                _ => "fas fa-circle"
            };
        }

        private static string GetActivityColor(string activityType)
        {
            return activityType switch
            {
                "PostView" => "#3498db",
                "Bookmark" => "#f39c12",
                "Comment" => "#2ecc71",
                "Like" => "#e74c3c",
                "Share" => "#9b59b6",
                _ => "#95a5a6"
            };
        }
    }
}