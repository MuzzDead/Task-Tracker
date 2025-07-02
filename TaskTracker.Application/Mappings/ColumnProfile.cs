using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
