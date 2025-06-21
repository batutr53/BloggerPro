using BloggerPro.Application.DTOs.Notification;
using BloggerPro.Shared.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloggerPro.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<DataResult<NotificationDto>> GetNotificationByIdAsync(Guid id);
        Task<DataResult<List<NotificationDto>>> GetUserNotificationsAsync(Guid userId, bool onlyUnread = false, int page = 1, int pageSize = 20);
        Task<Result> MarkAsReadAsync(Guid notificationId, Guid userId);
        Task<Result> MarkAllAsReadAsync(Guid userId);
        Task<DataResult<int>> GetUnreadCountAsync(Guid userId);
        Task<DataResult<NotificationDto>> CreateNotificationAsync(CreateNotificationDto notificationDto);
    }
}
