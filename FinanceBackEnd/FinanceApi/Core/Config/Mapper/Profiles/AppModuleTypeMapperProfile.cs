using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class AppModuleTypeMapperProfile : BaseEntityMapperProfile<AppModuleType, AppModuleTypeDto>
{
    public AppModuleTypeMapperProfile()
        : base()
    {
    }
}
