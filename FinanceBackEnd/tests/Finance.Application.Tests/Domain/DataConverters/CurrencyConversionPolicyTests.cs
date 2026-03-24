using Finance.Domain.Policies;
using Finance.Domain.Models.Currencies;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Tests.Domain.DataConverters;

public class CurrencyConversionPolicyTests
{
    private readonly CurrencyConversionPolicy _policy = new();

    private static CurrencyExchangeRate MakeRate(Guid baseId, Guid quoteId, decimal buyRate, decimal sellRate) =>
        new() { Id = Guid.NewGuid(), BaseCurrencyId = baseId, QuoteCurrencyId = quoteId, BuyRate = buyRate, SellRate = sellRate, TimeStamp = DateTime.UtcNow };

    [Fact]
    public void Apply_WhenSourceIsBase_DividesByBuyRate()
    {
        var baseId = Guid.NewGuid();
        var quoteId = Guid.NewGuid();
        var rate = MakeRate(baseId, quoteId, buyRate: 1000m, sellRate: 1100m);

        Money result = _policy.Apply(new Money(5000m), baseId, rate);

        Assert.Equal(5m, (decimal)result);
    }

    [Fact]
    public void Apply_WhenSourceIsQuote_MultipliesBySellRate()
    {
        var baseId = Guid.NewGuid();
        var quoteId = Guid.NewGuid();
        var rate = MakeRate(baseId, quoteId, buyRate: 1000m, sellRate: 1100m);

        Money result = _policy.Apply(new Money(5m), quoteId, rate);

        Assert.Equal(5500m, (decimal)result);
    }

    [Fact]
    public void Apply_NaturalAndReverseAreSymmetric()
    {
        var baseId = Guid.NewGuid();
        var quoteId = Guid.NewGuid();
        var rate = MakeRate(baseId, quoteId, buyRate: 1000m, sellRate: 1000m);

        Money forward = _policy.Apply(new Money(10m), quoteId, rate); // 10 * 1000 = 10000
        Money backward = _policy.Apply(forward, baseId, rate);          // 10000 / 1000 = 10

        Assert.Equal(10m, (decimal)backward);
    }
}
