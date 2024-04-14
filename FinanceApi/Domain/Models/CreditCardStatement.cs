using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class CreditCardStatement : Entity<Guid>
{
    public Guid? CreditCardId { get; set; }

    public virtual CreditCard CreditCard { get; set; }

    public DateTime ClosureDate { get; set; }

    public DateTime ExpiringDate { get; set; }
}
