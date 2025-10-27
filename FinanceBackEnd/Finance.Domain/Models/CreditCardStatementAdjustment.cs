using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCardStatementAdjustment : Entity<Guid>
{
    public Guid CreditCardStatementId { get; set; }
    public virtual CreditCardStatement CreditCardStatement { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
