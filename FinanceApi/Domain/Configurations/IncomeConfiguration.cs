using FinanceApi.Domain.Models;
using FinanceApi.Domain.TypeConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

public class IncomeConfiguration : IEntityTypeConfiguration<Income>
{
    public void Configure(EntityTypeBuilder<Income> builder)
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
    }
}
