namespace Finance.Application.Dtos.IOLInvestmentAssetTypes;

public record IOLInvestmentAssetTypeDto : Dto<ushort>
{
    public IOLInvestmentAssetTypeDto()
        : base()
    {
    }

    public string Name { get; set; } = string.Empty;
}
