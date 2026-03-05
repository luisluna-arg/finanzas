using Finance.Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations.Base;

public abstract class AuditedEntityConfiguration<T, TId> : IEntityTypeConfiguration<T>
    where T : AuditedEntity<TId>

{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
    }
}
