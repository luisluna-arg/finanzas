using Finance.Domain.Models.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models.CreditCards;

public class CreditCardPayment : Entity<Guid>
{
    public Guid StatementId { get; set; }
    public virtual CreditCardStatement Statement { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public Money Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Other;
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
}

public enum PaymentMethod
{
    Other = 0,
    BankTransfer = 1,
    Card = 2,
    Cash = 3
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2
}
