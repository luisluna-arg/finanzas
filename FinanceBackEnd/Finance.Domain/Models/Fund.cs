using Finance.Domain.Models.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models;

public class Fund : AuditedEntity<Guid>
{
    public Guid BankId { get; set; }
    public Guid CurrencyId { get; set; }
    public virtual Bank? Bank { get; set; }
    public virtual Currency? Currency { get; set; }
    public DateTime TimeStamp { get; set; }
    public Money Amount { get; set; } = 0m;
    public bool DailyUse { get; set; }
}
