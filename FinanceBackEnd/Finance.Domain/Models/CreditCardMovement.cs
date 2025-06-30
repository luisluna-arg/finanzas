using Finance.Domain.SpecialTypes;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCardMovement : Entity<Guid>
{
    public Guid CreditCardId { get; set; }
    public virtual CreditCard CreditCard { get; set; } = default!;
    required public DateTime TimeStamp { get; set; }
    required public DateTime PlanStart { get; set; }
    required public string Concept { get; set; }
    required public ushort PaymentNumber { get; set; } = 1;
    required public ushort PlanSize { get; set; } = 1;
    required public Money Amount { get; set; } = 0;
    required public Money AmountDollars { get; set; } = 0;
}
