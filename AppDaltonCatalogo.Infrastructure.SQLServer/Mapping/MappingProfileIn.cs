using AppDaltonCatalogo.Infrastructure.SQL.Dtos.Auth;
using AutoMapper;

namespace AppDaltonCatalogo.Infrastructure.SQL.Mapping
{
    public class MappingProfileIn : Profile
    {
        public MappingProfileIn()
        {
            CreateMap<LoginDto, strAuthLogin>();
        }
    }
}
