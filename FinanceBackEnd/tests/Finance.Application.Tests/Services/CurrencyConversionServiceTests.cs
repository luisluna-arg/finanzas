using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.Policies;
using Finance.Domain.SpecialTypes;
using Microsoft.Extensions.Caching.Memory;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Services;

public class CurrencyConversionServiceTests : QueryHandlerBaseTests
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly IMemoryCache _cache;
    private readonly CurrencyConversionService _sut;

    public CurrencyConversionServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _sut = new CurrencyConversionService(_dispatcher.Object, _dbContext, new CurrencyConversionPolicy(), _cache);
    }

    public override void Dispose()
    {
        base.Dispose();
        _cache.Dispose();
    }

    private void SetupDispatcherRates(List<CurrencyExchangeRate> rates)
    {
        _dispatcher
            .Setup(d => d.DispatchQueryAsync<List<CurrencyExchangeRate>>(It.IsAny<IQuery<List<CurrencyExchangeRate>>>()))
            .ReturnsAsync(DataResult<List<CurrencyExchangeRate>>.Success(rates));
    }

    private static CurrencyExchangeRate MakeRate(Guid baseId, Guid quoteId, decimal buyRate, decimal sellRate) =>
        new() { Id = Guid.NewGuid(), BaseCurrencyId = baseId, QuoteCurrencyId = quoteId, BuyRate = buyRate, SellRate = sellRate, TimeStamp = DateTime.UtcNow };

    private static IAmountHolder Holder(Guid currencyId, decimal amount) =>
        new SimpleHolder { CurrencyId = currencyId, Amount = new Money(amount) };

    // --- Convert ---

    [Fact]
    public async Task Convert_SameCurrency_ReturnsOriginalAmount()
    {
        var id = Guid.NewGuid();
        var result = await _sut.Convert(Holder(id, 500m), id);

        Assert.Equal(500m, (decimal)result);
        _dispatcher.Verify(d => d.DispatchQueryAsync<List<CurrencyExchangeRate>>(It.IsAny<IQuery<List<CurrencyExchangeRate>>>()), Times.Never);
    }

    [Fact]
    public async Task Convert_SourceIsBase_DividesByBuyRate()
    {
        var arsId = Guid.NewGuid();
        var usdId = Guid.NewGuid();
        var rate = MakeRate(arsId, usdId, buyRate: 1000m, sellRate: 1100m);
        SetupDispatcherRates([rate]);

        var result = await _sut.Convert(Holder(arsId, 5000m), usdId);

        Assert.Equal(5m, (decimal)result);
    }

    [Fact]
    public async Task Convert_SourceIsQuote_MultipliesBySellRate()
    {
        var arsId = Guid.NewGuid();
        var usdId = Guid.NewGuid();
        var rate = MakeRate(arsId, usdId, buyRate: 1000m, sellRate: 1100m);
        SetupDispatcherRates([rate]);

        var result = await _sut.Convert(Holder(usdId, 5m), arsId);

        Assert.Equal(5500m, (decimal)result);
    }

    [Fact]
    public async Task Convert_NoMatchingRate_ReturnsZero()
    {
        var arsId = Guid.NewGuid();
        var usdId = Guid.NewGuid();
        SetupDispatcherRates([]);

        var result = await _sut.Convert(Holder(arsId, 1000m), usdId);

        Assert.Equal(0m, (decimal)result);
    }

    [Fact]
    public async Task Convert_SelectsRateMatchingTheRequestedPair()
    {
        var arsId = Guid.NewGuid();
        var usdId = Guid.NewGuid();
        var eurId = Guid.NewGuid();
        var arsUsd = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrencyId = arsId, QuoteCurrencyId = usdId, BuyRate = 1000m, SellRate = 1100m, TimeStamp = DateTime.UtcNow };
        var arsEur = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrencyId = arsId, QuoteCurrencyId = eurId, BuyRate = 500m, SellRate = 550m, TimeStamp = DateTime.UtcNow };
        SetupDispatcherRates([arsUsd, arsEur]);

        var result = await _sut.Convert(Holder(arsId, 5000m), usdId);

        Assert.Equal(5m, (decimal)result); // 5000 / 1000 using arsUsd pair, not arsEur
    }

    // --- ConvertCollection ---

    [Fact]
    public async Task ConvertCollection_SameCurrency_PassesThrough()
    {
        var id = Guid.NewGuid();
        SetupDispatcherRates([]);

        var result = await _sut.ConvertCollection([Holder(id, 100m), Holder(id, 200m)], id);

        Assert.Equal([100m, 200m], result.Select(m => (decimal)m));
    }

    [Fact]
    public async Task ConvertCollection_MixedCurrencies_ConvertsCorrectly()
    {
        var arsId = Guid.NewGuid();
        var usdId = Guid.NewGuid();
        var rate = MakeRate(arsId, usdId, buyRate: 1000m, sellRate: 1100m);
        SetupDispatcherRates([rate]);

        var result = (await _sut.ConvertCollection([Holder(arsId, 3000m), Holder(usdId, 5m), Holder(arsId, 1000m)], arsId)).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(3000m, (decimal)result[0]);   // same currency
        Assert.Equal(5500m, (decimal)result[1]);   // usd→ars: 5 * 1100
        Assert.Equal(1000m, (decimal)result[2]);   // same currency
    }

    [Fact]
    public async Task ConvertCollection_NoRate_ReturnsZeroForThatItem()
    {
        var arsId = Guid.NewGuid();
        var usdId = Guid.NewGuid();
        SetupDispatcherRates([]);

        var result = await _sut.ConvertCollection([Holder(usdId, 50m)], arsId);

        Assert.Equal(0m, (decimal)result.Single());
    }

    [Fact]
    public async Task Convert_CachePreventsSecondDispatch()
    {
        var arsId = Guid.NewGuid();
        var usdId = Guid.NewGuid();
        var rate = MakeRate(arsId, usdId, buyRate: 1000m, sellRate: 1100m);
        SetupDispatcherRates([rate]);

        await _sut.Convert(Holder(arsId, 1000m), usdId);
        await _sut.Convert(Holder(arsId, 2000m), usdId);

        _dispatcher.Verify(d => d.DispatchQueryAsync<List<CurrencyExchangeRate>>(It.IsAny<IQuery<List<CurrencyExchangeRate>>>()), Times.Once);
    }

    private sealed class SimpleHolder : IAmountHolder
    {
        public Guid CurrencyId { get; set; }
        public Money Amount { get; set; }
    }
}
