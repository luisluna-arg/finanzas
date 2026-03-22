using Finance.Application.Queries.Catalog;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models.Frequencies;

namespace Finance.Application.Tests.Queries.Catalog;

public class GetCatalogFrequenciesQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task GetCatalogFrequencies_ReturnsAllFrequencies()
    {
        await _dbContext.Frequency.AddRangeAsync(
            new Frequency { Id = FrequencyEnum.Monthly, Name = "Monthly" },
            new Frequency { Id = FrequencyEnum.Annual, Name = "Annual" }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogFrequenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogFrequenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task GetCatalogFrequencies_ReturnsFrequenciesOrderedByName()
    {
        await _dbContext.Frequency.AddRangeAsync(
            new Frequency { Id = FrequencyEnum.Weekly, Name = "Weekly" },
            new Frequency { Id = FrequencyEnum.Annual, Name = "Annual" },
            new Frequency { Id = FrequencyEnum.Monthly, Name = "Monthly" }
        );
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogFrequenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogFrequenciesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("Annual", result.Data[0].Name);
        Assert.Equal("Monthly", result.Data[1].Name);
        Assert.Equal("Weekly", result.Data[2].Name);
    }

    [Fact]
    public async Task GetCatalogFrequencies_MapsEnumValueToId()
    {
        await _dbContext.Frequency.AddAsync(new Frequency { Id = FrequencyEnum.Monthly, Name = "Monthly" });
        await _dbContext.SaveChangesAsync();

        var handler = new GetCatalogFrequenciesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetCatalogFrequenciesQuery(), default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data);
        Assert.Equal((int)FrequencyEnum.Monthly, item.Id);
        Assert.Equal("Monthly", item.Name);
    }
}
