using System.ComponentModel.DataAnnotations.Schema;
using FinanceApi.Core.SpecialTypes;

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

    required public decimal LastPrice { get; set; } = 0M;

    required public decimal AverageBuyPrice { get; set; } = 0M;

    required public decimal AverageReturnPercent { get; set; } = 0M;

    required public decimal AverageReturn { get; set; } = 0M;

    required public decimal Valued { get; set; } = 0M;

    required public virtual InvestmentAssetIOLType InvestmentAssetIOLType { get; set; }
}
