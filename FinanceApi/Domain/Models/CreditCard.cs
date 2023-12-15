using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class CreditCard : Entity<Guid>
{
    public Guid BankId { get; set; }

    public virtual Bank Bank { get; set; }

    required public string Name { get; set; }
}
