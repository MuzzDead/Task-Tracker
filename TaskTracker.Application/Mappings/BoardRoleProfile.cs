using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
