using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Interfaces;

namespace Finance.Domain.Models.BankCurrencies;

public class BankCurrency : Entity, IAuditedEntity
{
    public Guid BankId { get; set; }
    public Guid CurrencyId { get; set; }
    public virtual Bank? Bank { get; set; }
    public virtual Currency? Currency { get; set; }
    public bool DailyUse { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
