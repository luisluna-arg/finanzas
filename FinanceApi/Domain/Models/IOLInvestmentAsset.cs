using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class IOLInvestmentAsset : Entity<Guid>
{
    public IOLInvestmentAsset()
        : base()
    {
    }

    required public string Symbol { get; set; } = string.Empty;
    required public string Description { get; set; } = string.Empty;
    public virtual IOLInvestmentAssetType Type { get; set; }
}
