using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models.CreditCards;

public class CreditCardStatementImportTemplate : AuditedEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
    public string ConfigJson { get; set; } = string.Empty;
    public virtual ICollection<CreditCard> CreditCards { get; set; } = [];
}
