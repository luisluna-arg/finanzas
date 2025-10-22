using Finance.Domain.SpecialTypes;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCardMovement : Entity<Guid>
{
    public Guid CreditCardId { get; set; }
    public virtual CreditCard CreditCard { get; set; } = null!;
    public DateTime TimeStamp { get; set; }
    public DateTime PlanStart { get; set; }
    public string Concept { get; set; } = string.Empty;
    public ushort PaymentNumber { get; set; } = 1;
    public ushort PlanSize { get; set; } = 1;
    public Money Amount { get; set; } = 0;
    public Money AmountDollars { get; set; } = 0;

    public CreditCardMovement() { }
}
