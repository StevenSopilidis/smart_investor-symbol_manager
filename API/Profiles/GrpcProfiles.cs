using Application.Dtos;
using AutoMapper;

namespace API.Profiles
{
    public class GrpcProfiles : Profile
    {
        public GrpcProfiles() {
            CreateMap<PostSymbolRequest, CreateSymbolDto>();
            CreateMap<SymbolDto, Symbol>();
        }
    }
}