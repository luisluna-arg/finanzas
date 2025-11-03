using Finance.Domain.Models.Base;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models;

public class Income : AuditedEntity<Guid>, IAmountHolder
{
    public Guid BankId { get; set; }
    public Guid CurrencyId { get; set; }
    public virtual Bank? Bank { get; set; }
    public virtual Currency? Currency { get; set; }
    public DateTime TimeStamp { get; set; }
    public Money Amount { get; set; }
}
