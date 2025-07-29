using Finance.Domain.Models.Interfaces;

namespace Finance.Domain.Models.Base;

public abstract class AuditedEntity<TId> : Entity<TId>, IAuditedEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
