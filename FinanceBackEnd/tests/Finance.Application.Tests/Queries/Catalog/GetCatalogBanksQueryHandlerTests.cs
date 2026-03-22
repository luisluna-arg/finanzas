using Finance.Application.Queries.Catalog;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Banks;

namespace Finance.Application.Tests.Queries.Catalog;

public class GetCatalogBanksQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task GetCatalogBanks_ReturnsOnlyActivebanks()
    {
        await _dbContext.Bank.AddRangeAsync(
            new Bank { Id = Guid.NewGuid(), Name = "Active Bank", Deactivated = false },
            new Bank { Id = Guid.NewGuid(), Name = "Inactive Bank", Deactivated = true }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogBanksQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogBanksQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.All(result.Data, item => Assert.NotEqual("Inactive Bank", item.Name));
    }

    [Fact]
    public async Task GetCatalogBanks_ReturnsBanksOrderedByName()
    {
        await _dbContext.Bank.AddRangeAsync(
            new Bank { Id = Guid.NewGuid(), Name = "Zeta Bank", Deactivated = false },
            new Bank { Id = Guid.NewGuid(), Name = "Alpha Bank", Deactivated = false },
            new Bank { Id = Guid.NewGuid(), Name = "Midway Bank", Deactivated = false }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogBanksQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogBanksQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("Alpha Bank", result.Data[0].Name);
        Assert.Equal("Midway Bank", result.Data[1].Name);
        Assert.Equal("Zeta Bank", result.Data[2].Name);
    }

    [Fact]
    public async Task GetCatalogBanks_MapsBankNameToItemName()
    {
        var id = Guid.NewGuid();
        await _dbContext.Bank.AddAsync(new Bank { Id = id, Name = "My Bank", Deactivated = false });
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogBanksQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogBanksQuery(), default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data);
        Assert.Equal(id, item.Id);
        Assert.Equal("My Bank", item.Name);
    }

    [Fact]
    public async Task GetCatalogBanks_WhenNoBanks_ReturnsEmptyList()
    {
        var handler = new GetCatalogBanksQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogBanksQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }
}
