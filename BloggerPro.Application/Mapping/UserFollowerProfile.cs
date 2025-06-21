using AutoMapper;
using BloggerPro.Application.DTOs.User;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping
{
    public class UserFollowerProfile : Profile
    {
        public UserFollowerProfile()
        {
            CreateMap<UserFollower, UserFollowerResponseDto>()
                .ForMember(dest => dest.FollowerUsername, opt => opt.MapFrom(src => src.Follower.UserName))
                .ForMember(dest => dest.FollowerProfileImage, opt => opt.MapFrom(src => src.Follower.ProfileImage))
                .ForMember(dest => dest.FollowingUsername, opt => opt.MapFrom(src => src.Following.UserName))
                .ForMember(dest => dest.FollowingProfileImage, opt => opt.MapFrom(src => src.Following.ProfileImage));
        }
    }
}
