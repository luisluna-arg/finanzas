using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class Fund : Entity<Guid>
{
    public Guid BankId { get; set; }

    public Guid CurrencyId { get; set; }

    public virtual Bank? Bank { get; set; }

    public virtual Currency? Currency { get; set; }

    required public DateTime CreatedAt { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public Money Amount { get; set; }
}
