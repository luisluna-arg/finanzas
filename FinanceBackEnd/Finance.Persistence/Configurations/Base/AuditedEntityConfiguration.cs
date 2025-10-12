using Finance.Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations.Base;

public abstract class AuditedEntityConfiguration<T, TId> : IEntityTypeConfiguration<T>
    where T : AuditedEntity<TId>

{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder
            .Property(o => o.CreatedAt)
            .HasConversion(o => o.ToUniversalTime(), o => o);

        builder
            .Property(o => o.UpdatedAt)
            .HasConversion(
                v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null,
                v => v
            );
    }
}
