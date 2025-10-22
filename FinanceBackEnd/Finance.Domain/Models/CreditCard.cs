using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCard : Entity<Guid>
{
    public Guid BankId { get; set; }
    public virtual Bank? Bank { get; set; }
    public Guid? CreditCardStatementId { get; set; }
    public virtual CreditCardStatement? CreditCardStatement { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<CreditCardMovement> Movements { get; set; } = new List<CreditCardMovement>();
    public decimal UnappliedCredit { get; set; }
    public virtual ICollection<CreditCardPaymentPlan> PaymentPlans { get; set; } = new List<CreditCardPaymentPlan>();
    public virtual ICollection<CreditCardPayment> Payments { get; set; } = new List<CreditCardPayment>();

    public CreditCard()
    {
    }
}
