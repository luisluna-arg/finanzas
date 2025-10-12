using Finance.Domain.Models;
using Finance.Persistence.Configurations.Base;
using Finance.Persistence.TypeConverters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CurrencyExchangeRateConfiguration : AuditedEntityConfiguration<CurrencyExchangeRate, Guid>
{
    public override void Configure(EntityTypeBuilder<CurrencyExchangeRate> builder)
    {
        base.Configure(builder);

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
