using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCardPaymentPlan : Entity<Guid>
{
    public Guid CreditCardId { get; set; }
    public virtual CreditCard CreditCard { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public int TotalInstallments { get; set; }
    public DateTime CreatedAt { get; set; }
    public PlanStatus Status { get; set; } = PlanStatus.Active;
    public virtual ICollection<CreditCardInstallment> Installments { get; set; } = new List<CreditCardInstallment>();
}

public enum PlanStatus
{
    Active = 0,
    Completed = 1,
    Cancelled = 2
}
