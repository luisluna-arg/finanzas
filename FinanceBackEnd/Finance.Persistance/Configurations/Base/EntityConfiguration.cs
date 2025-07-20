using Finance.Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations.Base;

public abstract class EntityConfiguration<T, TId> : IEntityTypeConfiguration<T>
    where T : Entity<TId>
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
    }
}
