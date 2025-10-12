using Finance.Domain.SpecialTypes;
using Finance.Domain.Models;
using Finance.Persistence.TypeConverters;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class MovementConfiguration : AuditedEntityConfiguration<Movement, Guid>
{
    public override void Configure(EntityTypeBuilder<Movement> builder)
    {
        base.Configure(builder);

        builder
            .Property(o => o.TimeStamp)
            .HasConversion(o => o.ToUniversalTime(), o => o);

        builder
            .Property(o => o.Amount)
            .HasConversion(new MoneyConverter());

        builder
            .Property(o => o.Total)
            .HasConversion(money => ((decimal?)money), value => value.HasValue ? new Money(value.Value) : null);
    }
}
