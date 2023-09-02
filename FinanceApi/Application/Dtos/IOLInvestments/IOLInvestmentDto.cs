namespace FinanceApi.Application.Dtos.IOLInvestments;

public record IOLInvestmentDto : Dto<Guid>
{
    public IOLInvestmentDto()
        : base()
    {
    }

    public string Asset { get; set; } = string.Empty;

    public uint Alarms { get; set; } = 0;

    public uint Quantity { get; set; } = 0;

    public uint Assets { get; set; } = 0;

    public decimal DailyVariation { get; set; } = 0M;

    public decimal LastPrice { get; set; } = 0M;

    public decimal AverageBuyPrice { get; set; } = 0M;

    public decimal AverageReturnPercent { get; set; } = 0M;

    public decimal AverageReturn { get; set; } = 0M;

    public decimal Valued { get; set; } = 0M;

    public virtual Guid InvestmentAssetIOLTypeId { get; set; }
}
