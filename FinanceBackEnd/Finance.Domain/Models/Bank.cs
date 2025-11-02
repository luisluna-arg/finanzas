using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class Bank : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<CreditCard> CreditCards { get; set; } = [];
}
