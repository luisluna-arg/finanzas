using Finance.Application.Legacy.Dtos.AppModules;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.AppModules;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class AppModuleCatalogMapper : BaseMapper<AppModule, AppModuleCatalogDto>, IAppModuleCatalogMapper
{
    public AppModuleCatalogMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IAppModuleCatalogMapper : IMapper<AppModule, AppModuleCatalogDto>;
