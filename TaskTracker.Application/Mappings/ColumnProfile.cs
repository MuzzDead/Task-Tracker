using AutoMapper;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public class ColumnProfile : Profile
{
    public ColumnProfile()
    {
        CreateMap<Column, ColumnDto>().ReverseMap();
    }
}
