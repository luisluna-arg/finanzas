using Finance.Application.Queries.Catalog;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Queries.Catalog;

public class GetCatalogCurrenciesQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task GetCatalogCurrencies_ReturnsOnlyActiveCurrencies()
    {
        await _dbContext.Currency.AddRangeAsync(
            CurrencyFixture.Build(shortName: "JPY"),
            CurrencyFixture.Build(shortName: "EUR", deactivated: true)
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var seeded = result.Data.Where(x => x.Name is "JPY" or "EUR").ToList();
        Assert.Single(seeded);
        Assert.All(result.Data, item => Assert.NotEqual("EUR", item.Name));
    }

    [Fact]
    public async Task GetCatalogCurrencies_ReturnsCurrenciesOrderedByShortName()
    {
        await _dbContext.Currency.AddRangeAsync(
            CurrencyFixture.Build(shortName: "GBP"),
            CurrencyFixture.Build(shortName: "EUR"),
            CurrencyFixture.Build(shortName: "JPY")
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var seeded = result.Data.Where(x => x.Name is "EUR" or "GBP" or "JPY").ToList();
        Assert.Equal(3, seeded.Count);
        Assert.Equal("EUR", seeded[0].Name);
        Assert.Equal("GBP", seeded[1].Name);
        Assert.Equal("JPY", seeded[2].Name);
    }

    [Fact]
    public async Task GetCatalogCurrencies_MapsShortNameToItemName()
    {
        var currency = CurrencyFixture.Build(shortName: "EUR");
        await _dbContext.Currency.AddAsync(currency);
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var item = result.Data.Single(x => x.Id == currency.Id);
        Assert.Equal("EUR", item.Name);
    }

    [Fact]
    public async Task GetCatalogCurrencies_ReturnsPreseededCurrencies()
    {
        var handler = new GetCatalogCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Data, x => x.Name == "ARS");
        Assert.Contains(result.Data, x => x.Name == "USD");
    }
}
