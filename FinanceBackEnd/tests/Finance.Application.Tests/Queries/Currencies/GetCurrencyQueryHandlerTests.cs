using Finance.Application.Queries.Currencies;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Tests.Queries.Currencies;

public class GetCurrencyQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task ReturnsNull_WhenCurrencyNotFound()
    {
        var handler = new GetCurrencyQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCurrencyQuery { Id = Guid.NewGuid() }, default);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ReturnsCurrency_WithSymbolsIncluded()
    {
        var currencyId = Guid.NewGuid();
        await _dbContext.Currency.AddAsync(new Currency { Id = currencyId, Name = "Peso Argentino", ShortName = "ARS" });
        await _dbContext.CurrencySymbols.AddAsync(new CurrencySymbol { Id = Guid.NewGuid(), CurrencyId = currencyId, Symbol = "$" });
        await _dbContext.SaveChangesAsync();

        var handler = new GetCurrencyQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCurrencyQuery { Id = currencyId }, default);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(currencyId, result.Data.Id);
        Assert.Single(result.Data.Symbols);
        Assert.Equal("$", result.Data.Symbols.First().Symbol);
    }

    [Fact]
    public async Task ReturnsCurrency_WithoutSymbols_WhenNoneSeeded()
    {
        var currencyId = Guid.NewGuid();
        await _dbContext.Currency.AddAsync(new Currency { Id = currencyId, Name = "Dollar", ShortName = "USD" });
        await _dbContext.SaveChangesAsync();

        var handler = new GetCurrencyQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCurrencyQuery { Id = currencyId }, default);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Symbols);
    }
}
