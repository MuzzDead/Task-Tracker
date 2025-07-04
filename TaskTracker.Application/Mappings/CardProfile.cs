using AutoMapper;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public class CardProfile : Profile
{
    public CardProfile()
    {
        CreateMap<Card, CardDto>().ReverseMap();
    }
}
