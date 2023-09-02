using FinanceApi.Domain.Models;
using FinanceApi.Domain.TypeConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

public class IOLInvestmentConfiguration : IEntityTypeConfiguration<IOLInvestment>
{
    public void Configure(EntityTypeBuilder<IOLInvestment> builder)
    {
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
