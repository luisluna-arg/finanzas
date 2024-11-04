using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class IOLInvestmentAsset : Entity<Guid>
{
    public IOLInvestmentAsset()
        : base()
    {
    }

    public virtual ushort TypeId { get; set; }
    required public string Symbol { get; set; } = string.Empty;
    required public string Description { get; set; } = string.Empty;
    public virtual IOLInvestmentAssetType Type { get; set; }
}
