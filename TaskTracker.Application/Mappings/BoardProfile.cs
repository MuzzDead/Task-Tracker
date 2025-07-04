using AutoMapper;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public class BoardProfile : Profile
{
    public BoardProfile()
    {
        CreateMap<Board, BoardDto>().ReverseMap();
    }
}
