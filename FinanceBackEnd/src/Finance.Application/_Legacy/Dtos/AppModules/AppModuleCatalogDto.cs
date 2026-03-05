using Finance.Application.Legacy.Dtos.Base;

namespace Finance.Application.Legacy.Dtos.AppModules;

public record AppModuleCatalogDto : CatalogDto<Guid>
{
    public AppModuleCatalogDto()
    {
    }
}
