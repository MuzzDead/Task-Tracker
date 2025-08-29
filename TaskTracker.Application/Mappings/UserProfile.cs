using AutoMapper;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());

        CreateMap<UserDto, User>()
            .ForMember(dest => dest.AvatarId, opt => opt.Ignore());
    }
}
