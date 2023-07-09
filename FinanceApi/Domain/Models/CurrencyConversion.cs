using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class CurrencyConversion : Entity<Guid>
{
    public CurrencyConversion()
        : base()
    {
    }

    public Guid MovementId { get; set; }

    public Guid? CurrencyId { get; set; }

    public Movement Movement { get; set; }

    public Currency? Currency { get; set; }

    public decimal Amount { get; set; }
}
