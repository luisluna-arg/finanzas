using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCardPaymentAllocation : Entity<Guid>
{
    public Guid PaymentId { get; set; }
    public virtual CreditCardPayment Payment { get; set; } = default!;

    // allocation targets
    public Guid? StatementId { get; set; }
    public Guid? PaymentPlanId { get; set; }
    public Guid? InstallmentId { get; set; }

    public decimal Amount { get; set; }
    public string? AllocationType { get; set; }
}
