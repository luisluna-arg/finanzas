namespace FinanceApi.Application.Dtos.InvestmentAssetIOLTypes;

public record InvestmentAssetIOLTypeDto : Dto<ushort>
{
    public InvestmentAssetIOLTypeDto()
        : base()
    {
    }

    public string Name { get; set; } = string.Empty;
}
