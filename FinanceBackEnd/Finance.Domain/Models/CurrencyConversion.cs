using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CurrencyConversion() : Entity<Guid>()
{
    public Guid MovementId { get; set; }
    public Guid? CurrencyId { get; set; }
    public virtual Movement Movement { get; set; } = default!;
    public virtual Currency? Currency { get; set; } = default;
    public decimal Amount { get; set; }
}
