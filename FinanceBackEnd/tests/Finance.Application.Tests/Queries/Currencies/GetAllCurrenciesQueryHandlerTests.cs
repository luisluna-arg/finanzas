using Finance.Application.Queries.Currencies;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Tests.Queries.Currencies;

public class GetAllCurrenciesQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task GetAllCurrencies_ReturnsOnlyActiveCurrencies()
    {
        var active = CurrencyFixture.Build();
        var inactive = CurrencyFixture.Build(deactivated: true);
        await _dbContext.Currency.AddRangeAsync(active, inactive);
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Data, c => c.Id == inactive.Id);
        Assert.All(result.Data, c => Assert.False(c.Deactivated));
    }

    [Fact]
    public async Task GetAllCurrencies_WhenIncludeDeactivated_ReturnsAll()
    {
        var active = CurrencyFixture.Build();
        var inactive = CurrencyFixture.Build(deactivated: true);
        await _dbContext.Currency.AddRangeAsync(active, inactive);
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery { IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Data, c => c.Id == active.Id);
        Assert.Contains(result.Data, c => c.Id == inactive.Id);
    }

    [Fact]
    public async Task GetAllCurrencies_ReturnsCurrenciesOrderedByName()
    {
        var c1 = CurrencyFixture.Build(shortName: "ZZZ", name: "Zebra");
        var c2 = CurrencyFixture.Build(shortName: "GBP", name: "British Pound");
        var c3 = CurrencyFixture.Build(shortName: "JPY", name: "Japanese Yen");
        await _dbContext.Currency.AddRangeAsync(c1, c2, c3);
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var seeded = result.Data.Where(x => x.Id == c1.Id || x.Id == c2.Id || x.Id == c3.Id).ToList();
        Assert.Equal(3, seeded.Count);
        Assert.Equal("British Pound", seeded[0].Name);
        Assert.Equal("Japanese Yen", seeded[1].Name);
        Assert.Equal("Zebra", seeded[2].Name);
    }

    [Fact]
    public async Task GetAllCurrencies_IncludesSymbols()
    {
        var currency = CurrencyFixture.Build();
        await _dbContext.Currency.AddAsync(currency);
        await _dbContext.CurrencySymbols.AddAsync(
            new CurrencySymbol { Id = Guid.NewGuid(), CurrencyId = currency.Id, Symbol = "€" }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var item = result.Data.Single(x => x.Id == currency.Id);
        Assert.Single(item.Symbols);
        Assert.Equal("€", item.Symbols.First().Symbol);
    }

    [Fact]
    public async Task GetAllCurrencies_CurrencyWithNoSymbols_ReturnsEmptySymbolsCollection()
    {
        var currency = CurrencyFixture.Build();
        await _dbContext.Currency.AddAsync(currency);
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var item = result.Data.Single(x => x.Id == currency.Id);
        Assert.Empty(item.Symbols);
    }

    [Fact]
    public async Task GetAllCurrencies_AlwaysContainsPreseededCurrencies()
    {
        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Data, c => c.ShortName == "ARS");
        Assert.Contains(result.Data, c => c.ShortName == "USD");
    }
}
