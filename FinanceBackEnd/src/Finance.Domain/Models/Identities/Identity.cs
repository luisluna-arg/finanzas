using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models.Identities;

public class Identity : AuditedEntity<Guid>
{
    public IdentityProviderEnum Provider { get; set; }
    public string SourceId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
