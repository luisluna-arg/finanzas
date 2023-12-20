using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class AppModuleTypeMapperProfile : BaseMapperProfile<AppModuleType, AppModuleTypeDto>
{
    public AppModuleTypeMapperProfile()
        : base()
    {
        Map.ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Name}"));
    }
}
