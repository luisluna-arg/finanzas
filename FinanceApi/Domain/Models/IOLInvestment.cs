using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class IOLInvestment : Entity<Guid>
{
    public IOLInvestment()
        : base()
    {
    }

    required public IOLInvestmentAsset Asset { get; set; }
    required public DateTime TimeStamp { get; set; }
    required public uint Alarms { get; set; } = 0;

    required public uint Quantity { get; set; } = 0;

    required public uint Assets { get; set; } = 0;

    required public decimal DailyVariation { get; set; } = 0M;

    required public Money LastPrice { get; set; } = 0M;

    required public Money AverageBuyPrice { get; set; } = 0M;

    required public Money AverageReturnPercent { get; set; } = 0M;

    required public Money AverageReturn { get; set; } = 0M;

    required public Money Valued { get; set; } = 0M;
}
