using Finance.Application.Queries.Catalog;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Tests.Queries.Catalog;

public class GetCatalogCurrenciesQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task GetCatalogCurrencies_ReturnsOnlyActiveCurrencies()
    {
        await _dbContext.Currency.AddRangeAsync(
            new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS", Deactivated = false },
            new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD", Deactivated = true }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.All(result.Data, item => Assert.NotEqual("USD", item.Name));
    }

    [Fact]
    public async Task GetCatalogCurrencies_ReturnsCurrenciesOrderedByShortName()
    {
        await _dbContext.Currency.AddRangeAsync(
            new Currency { Id = Guid.NewGuid(), Name = "Euro", ShortName = "EUR", Deactivated = false },
            new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS", Deactivated = false },
            new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD", Deactivated = false }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("ARS", result.Data[0].Name);
        Assert.Equal("EUR", result.Data[1].Name);
        Assert.Equal("USD", result.Data[2].Name);
    }

    [Fact]
    public async Task GetCatalogCurrencies_MapsShortNameToItemName()
    {
        var id = Guid.NewGuid();
        await _dbContext.Currency.AddAsync(new Currency { Id = id, Name = "Peso Argentino", ShortName = "ARS", Deactivated = false });
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data);
        Assert.Equal(id, item.Id);
        Assert.Equal("ARS", item.Name);
    }

    [Fact]
    public async Task GetCatalogCurrencies_WhenNoCurrencies_ReturnsEmptyList()
    {
        var handler = new GetCatalogCurrenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogCurrenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }
}
