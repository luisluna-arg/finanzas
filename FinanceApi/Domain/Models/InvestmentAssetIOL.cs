using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Enums;

namespace FinanceApi.Domain.Models;

public class InvestmentAssetIOL : Entity
{
    public InvestmentAssetIOL()
        : base()
    {
    }

    required public string Asset { get; set; } = string.Empty;
    required public uint Alarms { get; set; } = 0;
    required public uint Quantity { get; set; } = 0;
    required public uint Assets { get; set; } = 0;
    required public decimal DailyVariation { get; set; } = 0M;
    required public Money LastPrice { get; set; } = 0M;
    required public Money AverageBuyPrice { get; set; } = 0M;
    required public Money AverageReturnPercent { get; set; } = 0M;
    required public Money AverageReturn { get; set; } = 0M;
    required public Money Valued { get; set; } = 0M;
    required public short AssetTypeId { get; set; }
    required public virtual InvestmentAssetIOLType AssetType { get; set; }
    public InvestmentAssetIOLTypeEnum InvestmentAssetIOLTypeEnum => (InvestmentAssetIOLTypeEnum)AssetTypeId;
}
