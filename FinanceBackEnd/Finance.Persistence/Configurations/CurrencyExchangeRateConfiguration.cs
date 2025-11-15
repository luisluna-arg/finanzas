using Finance.Domain.Models.Currencies;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CurrencyExchangeRateConfiguration : AuditedEntityConfiguration<CurrencyExchangeRate, Guid>
{
    public override void Configure(EntityTypeBuilder<CurrencyExchangeRate> builder)
    {
        base.Configure(builder);

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
