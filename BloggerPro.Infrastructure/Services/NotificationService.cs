using AutoMapper;
using BloggerPro.Application.DTOs.Notification;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DataResult<NotificationDto>> GetNotificationByIdAsync(Guid id)
        {
            var notification = await _unitOfWork.Notifications
                .FindByCondition(n => n.Id == id)
                .FirstOrDefaultAsync();

            if (notification == null)
                return new ErrorDataResult<NotificationDto>("Bildirim bulunamadı.");

            var result = _mapper.Map<NotificationDto>(notification);
            return new SuccessDataResult<NotificationDto>(result);
        }

        public async Task<DataResult<List<NotificationDto>>> GetUserNotificationsAsync(Guid userId, bool onlyUnread = false, int page = 1, int pageSize = 20)
        {
            var query = _unitOfWork.Notifications
                .FindByCondition(n => n.UserId == userId);

            if (onlyUnread)
                query = query.Where(n => !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = _mapper.Map<List<NotificationDto>>(notifications);
            return new SuccessDataResult<List<NotificationDto>>(result);
        }

        public async Task<Result> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await _unitOfWork.Notifications
                .FindByCondition(n => n.Id == notificationId && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (notification == null)
                return new ErrorResult("Bildirim bulunamadı veya erişim izniniz yok.");

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _unitOfWork.Notifications.UpdateAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }

            return new SuccessResult("Bildirim okundu olarak işaretlendi.");
        }


        public async Task<Result> MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _unitOfWork.Notifications
                .FindByCondition(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                await _unitOfWork.Notifications.UpdateAsync(notification);
            }

            if (unreadNotifications.Any())
                await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Tüm bildirimler okundu olarak işaretlendi.");
        }

        public async Task<DataResult<int>> GetUnreadCountAsync(Guid userId)
        {
            var count = await _unitOfWork.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return new SuccessDataResult<int>(count);
        }

        public async Task<DataResult<NotificationDto>> CreateNotificationAsync(CreateNotificationDto notificationDto)
        {
            var notification = _mapper.Map<Notification>(notificationDto);
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsRead = false;

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<NotificationDto>(notification);
            return new SuccessDataResult<NotificationDto>(result, "Bildirim oluşturuldu.");
        }
    }
}
