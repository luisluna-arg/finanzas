using Finance.Domain.SpecialTypes;
using Finance.Domain.Models;
using Finance.Persistance.TypeConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder
            .Property(o => o.CreatedAt)
            .HasConversion(o => o.ToUniversalTime(), o => o);

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
