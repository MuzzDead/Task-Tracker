using AutoMapper;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public class BoardRoleProfile : Profile
{
    public BoardRoleProfile()
    {
        CreateMap<BoardRole, BoardRoleDto>().ReverseMap();
    }
}
