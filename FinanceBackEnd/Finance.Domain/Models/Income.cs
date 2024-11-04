using Finance.Domain.SpecialTypes;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.Interfaces;

namespace Finance.Domain.Models;

public class Income : Entity<Guid>, IAmountHolder
{
    public Guid BankId { get; set; }

    public Guid CurrencyId { get; set; }

    public virtual Bank? Bank { get; set; }

    public virtual Currency? Currency { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime TimeStamp { get; set; }

    public Money Amount { get; set; }
}
