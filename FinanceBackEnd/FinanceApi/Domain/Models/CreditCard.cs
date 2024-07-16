using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class CreditCard : Entity<Guid>
{
    public Guid BankId { get; set; }

    public virtual Bank Bank { get; set; }

    public Guid? CreditCardStatementId { get; set; }

    public virtual CreditCardStatement? CreditCardStatement { get; set; }

    required public string Name { get; set; }

    public virtual ICollection<CreditCardMovement> Movements { get; set; } = new List<CreditCardMovement>();
}
