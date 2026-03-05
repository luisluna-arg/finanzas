using Finance.Application.Legacy.Dtos.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Legacy.Dtos.IOLInvestmentAssetTypes;

public record IOLInvestmentAssetTypeDto : Dto<IOLInvestmentAssetTypeEnum>
{
    public string Name { get; set; } = string.Empty;

    public IOLInvestmentAssetTypeDto()
    {
    }
}
