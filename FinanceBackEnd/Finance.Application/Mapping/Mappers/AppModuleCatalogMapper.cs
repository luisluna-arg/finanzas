using Finance.Application.Dtos.AppModules;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.AppModules;

namespace Finance.Application.Mapping.Mappers;

public class AppModuleCatalogMapper : BaseMapper<AppModule, AppModuleCatalogDto>, IAppModuleCatalogMapper
{
    public AppModuleCatalogMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IAppModuleCatalogMapper : IMapper<AppModule, AppModuleCatalogDto>;
