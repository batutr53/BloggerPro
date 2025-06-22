using AutoMapper;
using BloggerPro.Application.DTOs.Notification;
using BloggerPro.Application.DTOs.User;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Enums;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BloggerPro.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UserService> _logger;
        private const string UploadsFolder = "uploads/profile-images";

        public UserService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            IWebHostEnvironment environment,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _environment = environment;
            _logger = logger;
            
            // Ensure uploads directory exists
            var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsFolder);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
        }

        public async Task<DataResult<UserProfileDto>> GetUserProfileAsync(Guid userId, Guid? currentUserId = null)
        {
            var user = await _unitOfWork.Users
     .FindByCondition(u => u.Id == userId)
     .Include(u => u.Posts)
     .Include(u => u.Followers)
     .Include(u => u.Following)
     .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ErrorDataResult<UserProfileDto>($"User with ID {userId} not found");
            }

            var userProfile = _mapper.Map<UserProfileDto>(user);
            
            // Set post count
            userProfile.PostCount = user.Posts?.Count ?? 0;

            // Check if current user is following this user
            if (currentUserId.HasValue && currentUserId.Value != userId)
            {
                var isFollowing = await _unitOfWork.UserFollowers
                    .FindByCondition(uf => uf.FollowerId == currentUserId.Value && uf.FollowingId == userId)
                    .AnyAsync();
                
                userProfile.IsFollowing = isFollowing;
            }
            else if (currentUserId.HasValue && currentUserId.Value == userId)
            {
                userProfile.IsFollowing = null; // It's the same user
            }

            return new SuccessDataResult<UserProfileDto>(userProfile);
        }


        public async Task<Result> UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto)
        {
            var user = await _unitOfWork.Users.FindByCondition(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ErrorResult($"User with ID {userId} not found");
            }

            // Update fields if they are provided in the DTO
            if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) user.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.Bio)) user.Bio = dto.Bio;
            if (!string.IsNullOrWhiteSpace(dto.ProfileImageUrl)) user.ProfileImageUrl = dto.ProfileImageUrl;
            if (!string.IsNullOrWhiteSpace(dto.Location)) user.Location = dto.Location;
            if (dto.BirthDate.HasValue) user.BirthDate = dto.BirthDate;
            if (!string.IsNullOrWhiteSpace(dto.Website)) user.Website = dto.Website;
            if (!string.IsNullOrWhiteSpace(dto.FacebookUrl)) user.FacebookUrl = dto.FacebookUrl;
            if (!string.IsNullOrWhiteSpace(dto.TwitterUrl)) user.TwitterUrl = dto.TwitterUrl;
            if (!string.IsNullOrWhiteSpace(dto.InstagramUrl)) user.InstagramUrl = dto.InstagramUrl;
            if (!string.IsNullOrWhiteSpace(dto.LinkedInUrl)) user.LinkedInUrl = dto.LinkedInUrl;

            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Profile updated successfully");
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _unitOfWork.Users.FindByCondition(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ErrorResult("User not found");
            }

            // Verify current password
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);

            if (result != PasswordVerificationResult.Success)
            {
                return new ErrorResult("Current password is incorrect");
            }

            // Update password
            var newPasswordHash = passwordHasher.HashPassword(user, dto.NewPassword);
            user.PasswordHash = newPasswordHash;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Password changed successfully");
        }

        public async Task<DataResult<string>> UploadProfileImageAsync(Guid userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new ErrorDataResult<string>("No file uploaded");
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                return new ErrorDataResult<string>("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
            }

            // Validate file size (max 5MB)
            const int maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                return new ErrorDataResult<string>("File size exceeds the maximum limit of 5MB");
            }

            var user = await _unitOfWork.Users.FindByCondition(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ErrorDataResult<string>("User not found");
            }

            try
            {
                // Generate unique filename
                var fileName = $"profile_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(_environment.WebRootPath, UploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update user's profile image URL
                var fileUrl = $"/{UploadsFolder}/{fileName}";
                user.ProfileImageUrl = fileUrl;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessDataResult<string>(fileUrl, "Profile image uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image for user {UserId}", userId);
                return new ErrorDataResult<string>("An error occurred while uploading the file");
            }
        }

        public async Task<Result> FollowUserAsync(Guid followerId, Guid followingId)
        {
            if (followerId == followingId)
            {
                return new ErrorResult("You cannot follow yourself");
            }

            // Check if users exist
            var followerExists = await _unitOfWork.Users.AnyAsync(u => u.Id == followerId);
            var followingExists = await _unitOfWork.Users.AnyAsync(u => u.Id == followingId);

            if (!followerExists || !followingExists)
            {
                return new ErrorResult("One or both users not found");
            }

            // Check if already following
            var alreadyFollowing = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == followerId && uf.FollowingId == followingId)
                .AnyAsync();

            if (alreadyFollowing)
            {
                return new ErrorResult("You are already following this user");
            }


            // Create the follow relationship
            var userFollower = new UserFollower
            {
                FollowerId = followerId,
                FollowingId = followingId,
                FollowedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserFollowers.AddAsync(userFollower);
            
            // Create notification for the user being followed
            var notification = new Notification
            {
                UserId = followingId,
                Message = $"User started following you",
                Type = NotificationType.NewFollower.ToString(),
                RelatedEntityId = followerId,
                RelatedEntityType = nameof(User),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Successfully followed user");
        }


        public async Task<Result> UnfollowUserAsync(Guid followerId, Guid followingId)
        {
            if (followerId == followingId)
            {
                return new ErrorResult("Invalid operation");
            }

            var userFollower = await _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == followerId && uf.FollowingId == followingId)
                .FirstOrDefaultAsync();

            if (userFollower == null)
            {
                return new ErrorResult("You are not following this user");
            }

            await _unitOfWork.UserFollowers.DeleteAsync(userFollower);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Successfully unfollowed user");
        }


        public async Task<DataResult<List<UserFollowerDto>>> GetUserFollowersAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            var followersQuery = _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowingId == userId)
                .Include(uf => uf.Follower)
                .OrderByDescending(uf => uf.FollowedAt);

            var totalCount = await followersQuery.CountAsync();
            var followers = await followersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(uf => new UserFollowerDto
                {
                    UserId = uf.FollowerId,
                    Username = uf.Follower.UserName,
                    FullName = $"{uf.Follower.FirstName} {uf.Follower.LastName}".Trim(),
                    ProfileImageUrl = uf.Follower.ProfileImageUrl,
                    FollowedAt = uf.FollowedAt
                })
                .ToListAsync();

            return new SuccessDataResult<List<UserFollowerDto>>(followers);
        }


        public async Task<DataResult<List<UserFollowerDto>>> GetUserFollowingAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            var followingQuery = _unitOfWork.UserFollowers
                .FindByCondition(uf => uf.FollowerId == userId)
                .Include(uf => uf.Following)
                .OrderByDescending(uf => uf.FollowedAt);

            var totalCount = await followingQuery.CountAsync();
            var following = await followingQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(uf => new UserFollowerDto
                {
                    UserId = uf.FollowingId,
                    Username = uf.Following.UserName,
                    FullName = $"{uf.Following.FirstName} {uf.Following.LastName}".Trim(),
                    ProfileImageUrl = uf.Following.ProfileImageUrl,
                    FollowedAt = uf.FollowedAt
                })
                .ToListAsync();

            return new SuccessDataResult<List<UserFollowerDto>>(following);
        }


        public async Task<DataResult<List<NotificationDto>>> GetUserNotificationsAsync(Guid userId, bool onlyUnread = false, int page = 1, int pageSize = 20)
        {
            var query = _unitOfWork.Notifications
                .FindByCondition(n => n.UserId == userId);

            if (onlyUnread)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    RelatedEntityId = n.RelatedEntityId,
                    RelatedEntityType = n.RelatedEntityType,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return new SuccessDataResult<List<NotificationDto>>(notifications);
        }


        public async Task<Result> MarkNotificationAsReadAsync(Guid userId, Guid notificationId)
        {
            var notification = await _unitOfWork.Notifications
                .FindByCondition(n => n.Id == notificationId && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                return new ErrorResult("Notification not found");
            }


            if (notification.IsRead)
            {
                return new SuccessResult("Notification is already marked as read");
            }


            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Notification marked as read");
        }


        public async Task<Result> MarkAllNotificationsAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _unitOfWork.Notifications
                .FindByCondition(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any())
            {
                return new SuccessResult("No unread notifications found");
            }

            var now = DateTime.UtcNow;
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = now;
            }

            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult($"Marked {unreadNotifications.Count} notifications as read");
        }
    }

}