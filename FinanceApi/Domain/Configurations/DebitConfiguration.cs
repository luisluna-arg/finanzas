using FinanceApi.Domain.Models;
using FinanceApi.Domain.TypeConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

public class DebitConfiguration : IEntityTypeConfiguration<Debit>
{
    public void Configure(EntityTypeBuilder<Debit> builder)
    {
        builder
            .Property(o => o.TimeStamp)
            .HasConversion(o => o.ToUniversalTime(), o => o);

        builder
            .Property(o => o.Amount)
            .HasConversion(new MoneyConverter());

        builder
            .Property(o => o.AmountDollars)
            .HasConversion(new MoneyConverter());
    }
}
