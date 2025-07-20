namespace Finance.Domain.Models.Base;

public abstract class AuditedEntity<TId> : Entity<TId>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
