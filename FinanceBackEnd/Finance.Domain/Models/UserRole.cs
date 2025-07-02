using Finance.Domain.Enums;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class UserRole : AuditedEntity<Guid>
{
    public required User User { get; set; }
    public required Guid UserId { get; set; }
    public required Role Role { get; set; }
    public required RoleEnum RoleId { get; set; }
}