using Finance.Domain.Models;
using Finance.Persistence.TypeConverters;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class IOLInvestmentConfiguration : AuditedEntityConfiguration<IOLInvestment, Guid>
{
    public override void Configure(EntityTypeBuilder<IOLInvestment> builder)
    {
        base.Configure(builder);
        builder
            .Property(o => o.LastPrice)
            .HasConversion(new MoneyConverter());

        builder
            .Property(o => o.AverageBuyPrice)
            .HasConversion(new MoneyConverter());

        builder
            .Property(o => o.AverageReturnPercent)
            .HasConversion(new MoneyConverter());

        builder
            .Property(o => o.AverageReturn)
            .HasConversion(new MoneyConverter());

        builder
            .Property(o => o.Valued)
            .HasConversion(new MoneyConverter());
    }
}
