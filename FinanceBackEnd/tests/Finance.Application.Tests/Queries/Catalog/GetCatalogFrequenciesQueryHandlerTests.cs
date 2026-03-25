using Finance.Application.Queries.Catalog;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Tests.Queries.Catalog;

public class GetCatalogFrequenciesQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task GetCatalogFrequencies_ReturnsAllFrequencies()
    {
        var handler = new GetCatalogFrequenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogFrequenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(Enum.GetValues<FrequencyEnum>().Length, result.Data.Count);
    }

    [Fact]
    public async Task GetCatalogFrequencies_ReturnsFrequenciesOrderedByName()
    {
        var handler = new GetCatalogFrequenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogFrequenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var names = result.Data.Select(x => x.Name).ToList();
        Assert.Equal(names.OrderBy(n => n).ToList(), names);
    }

    [Fact]
    public async Task GetCatalogFrequencies_MapsEnumValueToId()
    {
        var handler = new GetCatalogFrequenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogFrequenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var item = result.Data.Single(x => x.Id == (int)FrequencyEnum.Monthly);
        Assert.Equal("Monthly", item.Name);
    }
}
