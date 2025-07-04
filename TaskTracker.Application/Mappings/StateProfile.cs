using AutoMapper;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public class StateProfile : Profile
{
    public StateProfile()
    {
        CreateMap<State, StateDto>().ReverseMap();
    }
}
