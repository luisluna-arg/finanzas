using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCardPayment : Entity<Guid>
{
    public Guid CreditCardId { get; set; }
    public Guid? StatementId { get; set; }
    public Guid? PaymentPlanId { get; set; }
    public Guid? InstallmentId { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Other;
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
    public virtual CreditCard CreditCard { get; set; } = null!;
    public virtual CreditCardStatement Statement { get; set; } = null!;
    public virtual CreditCardPaymentPlan PaymentPlan { get; set; } = null!;
    public virtual CreditCardInstallment Installment { get; set; } = null!;
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
