using AutoMapper;
using BloggerPro.Application.DTOs.Chat;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderUserName, opt => opt.MapFrom(src => src.Sender.UserName))
                .ForMember(dest => dest.ReceiverUserName, opt => opt.MapFrom(src => src.Receiver.UserName));

            CreateMap<UserPresence, UserPresenceDto>();
        }
    }
}