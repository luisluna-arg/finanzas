using Finance.Application.Queries.Currencies;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Tests.Queries.Currencies;

public class GetAllCurrenciesQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task GetAllCurrencies_ReturnsOnlyActiveCurrencies()
    {
        await _dbContext.Currency.AddRangeAsync(
            new Currency { Id = Guid.NewGuid(), Name = "Peso Argentino", ShortName = "ARS", Deactivated = false },
            new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD", Deactivated = true }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.All(result.Data, c => Assert.False(c.Deactivated));
    }

    [Fact]
    public async Task GetAllCurrencies_WhenIncludeDeactivated_ReturnsAll()
    {
        await _dbContext.Currency.AddRangeAsync(
            new Currency { Id = Guid.NewGuid(), Name = "Peso Argentino", ShortName = "ARS", Deactivated = false },
            new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD", Deactivated = true }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery { IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task GetAllCurrencies_ReturnsCurrenciesOrderedByName()
    {
        await _dbContext.Currency.AddRangeAsync(
            new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD", Deactivated = false },
            new Currency { Id = Guid.NewGuid(), Name = "Euro", ShortName = "EUR", Deactivated = false },
            new Currency { Id = Guid.NewGuid(), Name = "Peso Argentino", ShortName = "ARS", Deactivated = false }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("Dollar", result.Data[0].Name);
        Assert.Equal("Euro", result.Data[1].Name);
        Assert.Equal("Peso Argentino", result.Data[2].Name);
    }

    [Fact]
    public async Task GetAllCurrencies_IncludesSymbols()
    {
        var currencyId = Guid.NewGuid();
        var currency = new Currency { Id = currencyId, Name = "Peso Argentino", ShortName = "ARS", Deactivated = false };
        await _dbContext.Currency.AddAsync(currency);
        await _dbContext.CurrencySymbols.AddAsync(
            new CurrencySymbol { Id = Guid.NewGuid(), CurrencyId = currencyId, Symbol = "$" }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data);
        Assert.Single(item.Symbols);
        Assert.Equal("$", item.Symbols.First().Symbol);
    }

    [Fact]
    public async Task GetAllCurrencies_CurrencyWithNoSymbols_ReturnsEmptySymbolsCollection()
    {
        await _dbContext.Currency.AddAsync(
            new Currency { Id = Guid.NewGuid(), Name = "Euro", ShortName = "EUR", Deactivated = false }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data);
        Assert.Empty(item.Symbols);
    }

    [Fact]
    public async Task GetAllCurrencies_WhenNoCurrencies_ReturnsEmptyList()
    {
        var handler = new GetAllCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }
}
