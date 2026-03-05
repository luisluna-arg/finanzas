using Finance.Application.Legacy.Dtos.AppModules;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.AppModules;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class AppModuleTypeMapper : BaseMapper<AppModuleType, AppModuleTypeDto>, IAppModuleTypeMapper
{
    public AppModuleTypeMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IAppModuleTypeMapper : IMapper<AppModuleType, AppModuleTypeDto>;
