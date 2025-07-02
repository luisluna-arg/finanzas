using Finance.Domain.Enums;

namespace Finance.Application.Dtos.IOLInvestmentAssetTypes;

public record IOLInvestmentAssetTypeDto : Dto<IOLInvestmentAssetTypeEnum>
{
    public IOLInvestmentAssetTypeDto()
        : base()
    {
    }

    public string Name { get; set; } = string.Empty;
}
