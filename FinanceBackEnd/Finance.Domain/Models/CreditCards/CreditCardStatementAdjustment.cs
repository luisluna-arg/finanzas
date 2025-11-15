using Finance.Domain.Models.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models.CreditCards;

public class CreditCardStatementAdjustment : Entity<Guid>
{
    public Guid CreditCardStatementId { get; set; }
    public virtual CreditCardStatement CreditCardStatement { get; set; } = null!;
    public Money Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
