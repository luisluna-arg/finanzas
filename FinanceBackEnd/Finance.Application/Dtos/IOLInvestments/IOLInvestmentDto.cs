using Finance.Application.Dtos.IOLInvestmentAssets;

namespace Finance.Application.Dtos.IOLInvestments;

public record IOLInvestmentDto : Dto<Guid>
{
    public IOLInvestmentDto()
        : base()
    {
    }

    required public DateTime CreatedAt { get; set; }

    required public DateTime TimeStamp { get; set; }

    public IOLInvestmentAssetDto Asset { get; set; }

    public uint Alarms { get; set; } = 0;

    public uint Quantity { get; set; } = 0;

    public uint Assets { get; set; } = 0;

    public decimal DailyVariation { get; set; } = 0M;

    public decimal LastPrice { get; set; } = 0M;

    public decimal AverageBuyPrice { get; set; } = 0M;

    public decimal AverageReturnPercent { get; set; } = 0M;

    public decimal AverageReturn { get; set; } = 0M;

    public decimal Valued { get; set; } = 0M;
}
