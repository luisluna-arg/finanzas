using Finance.Domain.Models.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Domain.Models.Banks;

public class Bank : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<CreditCard> CreditCards { get; set; } = [];
}
