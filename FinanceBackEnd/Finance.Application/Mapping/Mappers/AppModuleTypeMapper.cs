using Finance.Domain.Models;
using Finance.Application.Dtos.AppModules;
using Finance.Application.Mapping.Base;

namespace Finance.Application.Mapping.Mappers;

public class AppModuleTypeMapper : BaseMapper<AppModuleType, AppModuleTypeDto>, IAppModuleTypeMapper
{
    public AppModuleTypeMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IAppModuleTypeMapper : IMapper<AppModuleType, AppModuleTypeDto>
{
}
