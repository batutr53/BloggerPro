using AutoMapper;
using BloggerPro.Application.DTOs.TeamMember;
using BloggerPro.Domain.Entities;

namespace BloggerPro.Application.Mapping;

public class TeamMemberProfile : Profile
{
    public TeamMemberProfile()
    {
        CreateMap<TeamMember, TeamMemberListDto>();
        CreateMap<TeamMemberCreateDto, TeamMember>();
        CreateMap<TeamMemberUpdateDto, TeamMember>();
    }
}