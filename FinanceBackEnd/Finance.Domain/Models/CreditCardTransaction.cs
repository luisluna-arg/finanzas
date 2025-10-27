using Finance.Domain.Models.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models;

public class CreditCardTransaction : Entity<Guid>
{
    public Guid CreditCardId { get; set; }
    public virtual CreditCard CreditCard { get; set; } = null!;
    public Guid? StatementTransactionId { get; set; }
    public virtual CreditCardStatementTransaction? StatementTransaction { get; set; }
    public DateTime Timestamp { get; set; }
    public CreditCardTransactionType TransactionType { get; set; }
    public string Concept { get; set; } = string.Empty;
    public Money Amount { get; set; } = 0;
    public string? Reference { get; set; }
}

public enum CreditCardTransactionType
{
    Purchase = 0,
    Refund = 1,
    Payment = 2,
    Adjustment = 3
}
