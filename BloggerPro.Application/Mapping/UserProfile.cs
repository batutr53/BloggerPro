using AutoMapper;
using BloggerPro.Application.DTOs.User;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();

            // User -> UserProfileDto
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(dest => dest.MemberSince, opt => opt.MapFrom(src => src.CreatedAt))
                   .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
    .ForMember(dest => dest.FollowerCount, opt => opt.MapFrom(src => src.Followers.Count))
    .ForMember(dest => dest.FollowingCount, opt => opt.MapFrom(src => src.Following.Count));

            // UpdateProfileDto -> User (only non-null fields)
            CreateMap<UpdateProfileDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
