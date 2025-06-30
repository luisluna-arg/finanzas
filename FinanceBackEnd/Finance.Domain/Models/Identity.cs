using Finance.Domain.Enums;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class Identity : AuditedEntity<Guid>
{
    public IdentityProviderEnum Provider { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}