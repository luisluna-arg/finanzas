using Finance.Domain.Models;
using Finance.Persistance.TypeConverters;
using Finance.Persistance.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class IncomeConfiguration : AuditedEntityConfiguration<Income, Guid>
{
    public override void Configure(EntityTypeBuilder<Income> builder)
    {
        base.Configure(builder);

        builder
            .Property(o => o.TimeStamp)
            .HasConversion(o => o.ToUniversalTime(), o => o);

        builder
            .Property(o => o.Amount)
            .HasConversion(new MoneyConverter());
    }
}
