using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class ResourceOwner : AuditedEntity<Guid>
{
    public required User User { get; set; }
    public Guid UserId { get; set; }
    public required Resource Resource { get; set; }
    public Guid ResourceId { get; set; }
}
