using AutoMapper;
using BloggerPro.Application.DTOs.User;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // User -> UserProfileDto
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(dest => dest.MemberSince, opt => opt.MapFrom(src => src.CreatedAt));

            // UpdateProfileDto -> User (only non-null fields)
            CreateMap<UpdateProfileDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
