using Finance.Domain.Models.CreditCards;

namespace Finance.Domain.Models.Base;

public abstract class CreditCardEntity : Entity<Guid>
{
    public Guid CreditCardId { get; set; }
    public virtual CreditCard CreditCard { get; set; } = default!;
}
