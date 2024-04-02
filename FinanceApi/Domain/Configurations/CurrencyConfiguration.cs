using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder
            .HasIndex(o => o.Name).IsUnique();

        builder
            .HasIndex(o => o.ShortName).IsUnique();

        builder
            .HasMany(o => o.BaseExchangeRates)
            .WithOne(o => o.BaseCurrency);

        builder
            .HasMany(o => o.QuoteExchangeRates)
            .WithOne(o => o.QuoteCurrency);
    }
}
