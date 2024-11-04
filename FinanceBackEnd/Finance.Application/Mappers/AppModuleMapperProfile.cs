using Finance.Application.Dtos.AppModules;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class AppModuleMapperProfile : BaseEntityMapperProfile<AppModule, AppModuleDto>
{
    public AppModuleMapperProfile()
        : base()
    {
    }
}
