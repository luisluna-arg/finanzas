using Finance.Application.Dtos.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Dtos.AppModules;

public record AppModuleTypeDto : CatalogDto<AppModuleTypeEnum>
{
    public AppModuleTypeDto()
        : base()
    {
    }
}
