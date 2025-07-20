using Finance.Domain.Enums;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class IOLInvestmentAsset() : Entity<Guid>()
{
    public virtual IOLInvestmentAssetTypeEnum TypeId { get; set; }
    public virtual Guid CurrencyId { get; set; } = default!;
    public string Symbol { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual IOLInvestmentAssetType Type { get; set; } = default!;
    public virtual Currency Currency { get; set; } = default!;
}
