using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models.CreditCards;

public class CreditCardInstallmentPattern : AuditedEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string RegexPattern { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
}
