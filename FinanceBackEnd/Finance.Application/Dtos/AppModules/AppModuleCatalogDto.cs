using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.AppModules;

public record AppModuleCatalogDto : CatalogDto<Guid>
{
    public AppModuleCatalogDto()
    {
    }
}