using Finance.Domain.Models.Base;
using Finance.Domain.Models.Identities;

namespace Finance.Domain.Models.Auth;

public class User : AuditedEntity<Guid>
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public ICollection<Role> Roles { get; set; } = [];
    public ICollection<Identity> Identities { get; set; } = [];
}
