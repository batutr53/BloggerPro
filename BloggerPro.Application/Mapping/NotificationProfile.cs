using AutoMapper;
using BloggerPro.Application.DTOs.Notification;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationResponseDto>();
            CreateMap<CreateNotificationDto, Notification>();
        }
    }
}
