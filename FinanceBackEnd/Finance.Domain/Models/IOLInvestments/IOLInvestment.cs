using Finance.Domain.Models.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models.IOLInvestments;

public class IOLInvestment() : AuditedEntity<Guid>
{
    public Guid AssetId { get; set; }
    public virtual IOLInvestmentAsset Asset { get; set; } = default!;
    public DateTime TimeStamp { get; set; }
    public uint Alarms { get; set; } = 0;
    public uint Quantity { get; set; } = 0;
    public uint Assets { get; set; } = 0;
    public decimal DailyVariation { get; set; } = 0M;
    public Money LastPrice { get; set; } = 0M;
    public Money AverageBuyPrice { get; set; } = 0M;
    public Money AverageReturnPercent { get; set; } = 0M;
    public Money AverageReturn { get; set; } = 0M;
    public Money Valued { get; set; } = 0M;
}
