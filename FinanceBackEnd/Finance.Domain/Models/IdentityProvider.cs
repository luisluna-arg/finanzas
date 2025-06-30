using Finance.Domain.Enums;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class IdentityProvider : AuditedEntity<IdentityProviderEnum>
{
    public string Name { get; set; } = string.Empty;

    public override string ToString() => Name;
}
