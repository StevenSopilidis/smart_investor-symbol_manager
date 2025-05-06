using Application.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class DtoProfiles : Profile
    {
        public DtoProfiles() {
            CreateMap<CreateSymbolDto, Symbol>();
            CreateMap<Symbol, SymbolDto>();
        }
    }
}