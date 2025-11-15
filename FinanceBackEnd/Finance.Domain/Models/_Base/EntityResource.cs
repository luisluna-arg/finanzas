using Finance.Domain.Models.Auth;

namespace Finance.Domain.Models.Base;

public abstract class EntityResource<T, TId> : AuditedEntity<TId>
    where T : Entity<TId>, new()
{
    public Resource Resource { get; set; } = new();
    public Guid ResourceId { get; set; }
    public T ResourceSource { get; set; } = new();
    public TId ResourceSourceId { get; set; } = default!;

    public static TResult Default<TResult>()
         where TResult : EntityResource<T, TId>, new()
        => new TResult
        {
            Resource = new(),
            ResourceSource = new(),
            ResourceId = default,
            ResourceSourceId = default!
        };
}
