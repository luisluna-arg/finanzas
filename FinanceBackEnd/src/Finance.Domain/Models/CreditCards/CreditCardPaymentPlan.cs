using Finance.Domain.Models.Base;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Interfaces;

namespace Finance.Domain.Models.CreditCards;

public class CreditCardPaymentPlan : CreditCardEntity, IAuditedEntity
{
    public string BaseConcept { get; set; } = string.Empty;
    public int TotalInstallments { get; set; }
    public Guid CurrencyId { get; set; }
    public virtual Currency Currency { get; set; } = default!;
    public virtual ICollection<CreditCardTransaction> Transactions { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
