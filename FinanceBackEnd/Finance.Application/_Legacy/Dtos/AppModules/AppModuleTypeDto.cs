using Finance.Application.Legacy.Dtos.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Legacy.Dtos.AppModules;

public record AppModuleTypeDto : CatalogDto<AppModuleTypeEnum>
{
    public AppModuleTypeDto()
        : base()
    {
    }
}
