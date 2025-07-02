namespace Finance.Domain.Models.Base;

public abstract class EntityResource<T, TId> : AuditedEntity<TId>
    where T : Entity<TId>
{
    public required Resource Resource { get; set; }
    public Guid ResourceId { get; set; }
    public required T ResourceSource { get; set; }
    public Guid ResourceSourceId { get; set; }
}