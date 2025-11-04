using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCardIssuer : Entity<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<CreditCard> CreditCards { get; set; } = [];
}
