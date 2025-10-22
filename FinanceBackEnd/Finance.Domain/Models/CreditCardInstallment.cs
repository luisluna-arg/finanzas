using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCardInstallment : Entity<Guid>
{
    public Guid PaymentPlanId { get; set; }
    public virtual CreditCardPaymentPlan PaymentPlan { get; set; } = null!;
    public int Number { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public InstallmentStatus Status { get; set; } = InstallmentStatus.Pending;
    public Guid? PaymentId { get; set; }
    public virtual CreditCardPayment Payment { get; set; } = null!;
}

public enum InstallmentStatus
{
    Pending = 0,
    Paid = 1,
    Late = 2,
    Partial = 3
}
