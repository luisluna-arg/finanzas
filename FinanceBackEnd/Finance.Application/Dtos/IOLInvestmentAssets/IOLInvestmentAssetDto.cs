using Finance.Application.Dtos.Base;
using Finance.Application.Dtos.IOLInvestmentAssetTypes;
using Finance.Domain.Enums;

namespace Finance.Application.Dtos.IOLInvestmentAssets;

public record IOLInvestmentAssetDto : Dto<Guid>
{
    public IOLInvestmentAssetTypeEnum TypeId { get; set; } = default;
    public string Symbol { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IOLInvestmentAssetTypeDto Type { get; set; } = default!;

    public IOLInvestmentAssetDto()
    {
    }
}
