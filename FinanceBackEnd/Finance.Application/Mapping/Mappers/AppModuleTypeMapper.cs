using Finance.Application.Dtos.AppModules;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

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
