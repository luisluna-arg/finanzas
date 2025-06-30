using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCard() : Entity<Guid>()
{
    public Guid BankId { get; set; } = default!;
    public virtual Bank Bank { get; set; } = default!;
    public Guid? CreditCardStatementId { get; set; }
    public virtual CreditCardStatement? CreditCardStatement { get; set; } = default!;
    required public string Name { get; set; }
    public virtual ICollection<CreditCardMovement> Movements { get; set; } = [];
}
