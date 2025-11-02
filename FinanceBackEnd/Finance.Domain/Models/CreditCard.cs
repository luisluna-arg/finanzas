using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCard : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public decimal UnappliedCredit { get; set; }
    public Guid BankId { get; set; }
    public virtual Bank Bank { get; set; } = default!;
    public Guid? CurrentStatementId { get; set; }
    public virtual CreditCardStatement? CurrentStatement { get; set; }
    public Guid? CreditCardIssuerId { get; set; }
    public virtual CreditCardIssuer? CreditCardIssuer { get; set; }
    public virtual ICollection<CreditCardTransaction> Transactions { get; set; } = [];
    public virtual ICollection<CreditCardStatement> Statements { get; set; } = [];
    public virtual ICollection<CreditCardStatementTransaction> StatementTransactions { get; set; } = [];
    public virtual ICollection<CreditCardPayment> Payments { get; set; } = [];

    public CreditCard()
    {
    }
}
