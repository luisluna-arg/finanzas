using AutoMapper;
using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class AppModuleMapperProfile : Profile
{
    public AppModuleMapperProfile()
    {
        CreateMap<AppModule, AppModuleDto>();
    }
}
