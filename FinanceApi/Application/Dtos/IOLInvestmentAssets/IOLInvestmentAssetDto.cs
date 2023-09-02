namespace FinanceApi.Application.Dtos.IOLInvestmentAssets;

public record IOLInvestmentAssetDto : Dto<Guid>
{
    public IOLInvestmentAssetDto()
        : base()
    {
    }

    public string Symbol { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
