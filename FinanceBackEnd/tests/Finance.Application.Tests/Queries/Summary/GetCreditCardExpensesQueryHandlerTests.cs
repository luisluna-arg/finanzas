using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Queries.Summary;
using Finance.Application.Services;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Policies;
using Finance.Domain.SpecialTypes;
using Finance.Persistence.Constants;
using Microsoft.Extensions.Caching.Memory;

namespace Finance.Application.Tests.Queries.Summary;

public class GetCreditCardExpensesQueryHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly IMemoryCache _cache;
    private readonly CurrencyConversionService _currencyConverter;

    private static readonly Guid DefaultCurrencyId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);
    private static readonly Guid UsdCurrencyId = Guid.Parse(CurrencyConstants.DollarId);

    public GetCreditCardExpensesQueryHandlerTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _currencyConverter = new CurrencyConversionService(_dispatcher.Object, _dbContext, new CurrencyConversionPolicy(), _cache);
    }

    public override void Dispose()
    {
        base.Dispose();
        _cache.Dispose();
    }

    private GetCreditCardExpensesQueryHandler CreateHandler() =>
        new(_dispatcher.Object, _currencyConverter);

    private void SetupTransactions(List<CreditCardTransaction> transactions)
    {
        _dispatcher
            .Setup(d => d.DispatchQueryAsync<List<CreditCardTransaction>>(It.IsAny<IQuery<List<CreditCardTransaction>>>()))
            .ReturnsAsync(DataResult<List<CreditCardTransaction>>.Success(transactions));
    }

    private void SetupExchangeRates(List<CurrencyExchangeRate> rates)
    {
        _dispatcher
            .Setup(d => d.DispatchQueryAsync<List<CurrencyExchangeRate>>(It.IsAny<IQuery<List<CurrencyExchangeRate>>>()))
            .ReturnsAsync(DataResult<List<CurrencyExchangeRate>>.Success(rates));
    }

    [Fact]
    public async Task Execute_WhenNoTransactions_ReturnsZeroValue()
    {
        SetupTransactions([]);
        SetupExchangeRates([]);

        var result = await CreateHandler().ExecuteAsync(new GetCreditCardExpensesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(0m, result.Data.Value);
    }

    [Fact]
    public async Task Execute_WithDefaultCurrencyTransactions_ReturnsSumDirectly()
    {
        var transactions = new List<CreditCardTransaction>
        {
            new() { Id = Guid.NewGuid(), CurrencyId = DefaultCurrencyId, Amount = new Money(1000m) },
            new() { Id = Guid.NewGuid(), CurrencyId = DefaultCurrencyId, Amount = new Money(500m) },
        };
        SetupTransactions(transactions);
        SetupExchangeRates([]);

        var result = await CreateHandler().ExecuteAsync(new GetCreditCardExpensesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(1500m, result.Data.Value);
    }

    [Fact]
    public async Task Execute_WithForeignCurrencyTransactions_ConvertsToDefaultCurrency()
    {
        var transactions = new List<CreditCardTransaction>
        {
            new() { Id = Guid.NewGuid(), CurrencyId = UsdCurrencyId, Amount = new Money(100m) },
        };
        var rates = new List<CurrencyExchangeRate>
        {
            new()
            {
                BaseCurrencyId = DefaultCurrencyId,
                QuoteCurrencyId = UsdCurrencyId,
                BuyRate = 1000m,
                SellRate = 1100m,
                TimeStamp = DateTime.UtcNow,
            },
        };
        SetupTransactions(transactions);
        SetupExchangeRates(rates);

        var result = await CreateHandler().ExecuteAsync(new GetCreditCardExpensesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(110000m, result.Data.Value);
    }

    [Fact]
    public async Task Execute_WithMixedCurrencyTransactions_ConvertsAndSumsAll()
    {
        var transactions = new List<CreditCardTransaction>
        {
            new() { Id = Guid.NewGuid(), CurrencyId = DefaultCurrencyId, Amount = new Money(2000m) },
            new() { Id = Guid.NewGuid(), CurrencyId = UsdCurrencyId, Amount = new Money(50m) },
        };
        var rates = new List<CurrencyExchangeRate>
        {
            new()
            {
                BaseCurrencyId = DefaultCurrencyId,
                QuoteCurrencyId = UsdCurrencyId,
                BuyRate = 900m,
                SellRate = 1000m,
                TimeStamp = DateTime.UtcNow,
            },
        };
        SetupTransactions(transactions);
        SetupExchangeRates(rates);

        var result = await CreateHandler().ExecuteAsync(new GetCreditCardExpensesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(52000m, result.Data.Value);
    }
}
