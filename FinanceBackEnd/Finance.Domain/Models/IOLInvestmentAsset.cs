using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class IOLInvestmentAsset : Entity<Guid>
{
    public IOLInvestmentAsset()
        : base()
    {
    }

    public virtual ushort TypeId { get; set; }
    public virtual Guid CurrencyId { get; set; } = default!;
    required public string Symbol { get; set; } = string.Empty;
    required public string Description { get; set; } = string.Empty;
    public virtual IOLInvestmentAssetType Type { get; set; } = default!;
    public virtual Currency Currency { get; set; } = default!;
}
