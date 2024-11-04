using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CurrencyConversion : Entity<Guid>
{
    public CurrencyConversion()
        : base()
    {
    }

    public Guid MovementId { get; set; }

    public Guid? CurrencyId { get; set; }

    public virtual Movement Movement { get; set; }

    public virtual Currency? Currency { get; set; }

    public decimal Amount { get; set; }
}
