using Finance.Domain.Models;
using Finance.Persistance.TypeConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class CurrencyExchangeRateConfiguration : IEntityTypeConfiguration<CurrencyExchangeRate>
{
    public void Configure(EntityTypeBuilder<CurrencyExchangeRate> builder)
    {
        builder
            .Property(o => o.TimeStamp)
            .HasConversion(o => o.ToUniversalTime(), o => o);

        builder
            .Property(o => o.BuyRate)
            .HasConversion(new MoneyConverter());

        builder
            .Property(o => o.SellRate)
            .HasConversion(new MoneyConverter());

        builder
            .HasIndex(o => new { o.BaseCurrencyId, o.QuoteCurrencyId, o.TimeStamp }).IsUnique();

        builder
            .HasOne(o => o.BaseCurrency)
            .WithMany(o => o.BaseExchangeRates);

        builder
            .HasOne(o => o.QuoteCurrency)
            .WithMany(o => o.QuoteExchangeRates);
    }
}
