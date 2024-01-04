using FinanceApi.Application.Dtos.IOLInvestmentAssetTypes;

namespace FinanceApi.Application.Dtos.IOLInvestmentAssets;

public record IOLInvestmentAssetDto : Dto<Guid>
{
    public IOLInvestmentAssetDto()
        : base()
    {
    }

    public short TypeId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IOLInvestmentAssetTypeDto Type { get; set; }
}
