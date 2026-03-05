using Finance.Domain.Models.CreditCards;

namespace Finance.Domain.Models.Base;

public abstract class CreditCardStatementEntity : Entity<Guid>
{
    public Guid StatementId { get; set; }
    public virtual CreditCardStatement Statement { get; set; } = null!;
}
