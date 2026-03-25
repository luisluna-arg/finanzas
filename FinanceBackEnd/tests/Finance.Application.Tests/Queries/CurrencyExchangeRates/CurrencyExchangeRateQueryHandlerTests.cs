using Finance.Application.Queries.CurrencyExchangeRates;
using Finance.Application.Repositories;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Identities;
using FinanceBackEnd.Finance.Domain.Enums;

namespace Finance.Application.Tests.Queries.CurrencyExchangeRates;

public class CurrencyExchangeRateQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task GetAllCurrencyExchangeRates_FiltersByPairAndTimestampAndDeactivated()
    {
        var user = await CreateCurrentUserAsync();
        var ars = CurrencyFixture.Build(shortName: "ARS");
        var usd = CurrencyFixture.Build(shortName: "USD");
        var brl = CurrencyFixture.Build(shortName: "BRL");

        var matching = new CurrencyExchangeRate
        {
            Id = Guid.NewGuid(),
            BaseCurrency = ars,
            BaseCurrencyId = ars.Id,
            QuoteCurrency = usd,
            QuoteCurrencyId = usd.Id,
            BuyRate = 900m,
            SellRate = 910m,
            TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            Deactivated = false,
        };

        var rates = new[]
        {
            matching,
            new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 800m, SellRate = 810m, TimeStamp = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc), Deactivated = false },
            new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = brl, QuoteCurrencyId = brl.Id, BuyRate = 5m, SellRate = 6m, TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), Deactivated = false },
            new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 850m, SellRate = 860m, TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), Deactivated = true },
        };

        await _dbContext.CurrencyExchangeRate.AddRangeAsync(rates);
        await _dbContext.SaveChangesAsync();
        await GrantAccessAsync(user, rates);

        var handler = new GetAllCurrencyExchangeRatesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllCurrencyExchangeRatesQuery
        {
            IncludeDeactivated = false,
            BaseCurrencyId = ars.Id,
            QuoteCurrencyId = usd.Id,
            TimeStampStart = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc),
            TimeStampEnd = new DateTime(2025, 1, 20, 0, 0, 0, DateTimeKind.Utc),
        }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(matching.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetPaginatedCurrencyExchangeRates_ReturnsOrderedPageAndTotalCount()
    {
        var user = await CreateCurrentUserAsync();
        var ars = CurrencyFixture.Build(shortName: "ARS");
        var usd = CurrencyFixture.Build(shortName: "USD");

        var rates = new[]
        {
            new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 800m, SellRate = 810m, TimeStamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 850m, SellRate = 860m, TimeStamp = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc) },
            new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 900m, SellRate = 910m, TimeStamp = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc) },
        };

        await _dbContext.CurrencyExchangeRate.AddRangeAsync(rates);
        await _dbContext.SaveChangesAsync();
        await GrantAccessAsync(user, rates);

        var handler = new GetPaginatedCurrencyExchangeRatesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetPaginatedCurrencyExchangeRatesQuery
        {
            Page = 2,
            PageSize = 1,
            IncludeDeactivated = true,
        }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Data.TotalItems);
        var items = result.Data.Items.ToList();
        Assert.Single(items);
        Assert.Equal(850m, (decimal)items[0]!.BuyRate);
    }

    [Fact]
    public async Task GetCurrencyExchangeRate_ReturnsRepositoryResult()
    {
        var rate = new CurrencyExchangeRate { Id = Guid.NewGuid(), BuyRate = 900m, SellRate = 910m, TimeStamp = DateTime.UtcNow };
        var repository = new Mock<IRepository<CurrencyExchangeRate, Guid>>();
        repository.Setup(r => r.GetByIdAsync(rate.Id, It.IsAny<CancellationToken>())).ReturnsAsync(rate);

        var handler = new GetCurrencyExchangeRateQueryHandler(_dbContext, repository.Object);
        var result = await handler.ExecuteAsync(new GetCurrencyExchangeRateQuery { Id = rate.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(rate, result.Data);
    }

    [Fact]
    public async Task GetLatestCurrencyExchangeRates_ReturnsOnlyLatestPerPair()
    {
        var user = await CreateCurrentUserAsync();
        var ars = new Currency { Id = Guid.NewGuid(), Name = "Peso Argentino", ShortName = "ARS" };
        var usd = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };

        var older = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 800m, SellRate = 810m, TimeStamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var latest = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 900m, SellRate = 910m, TimeStamp = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc) };

        await _dbContext.CurrencyExchangeRate.AddRangeAsync(older, latest);
        await _dbContext.SaveChangesAsync();
        await GrantAccessAsync(user, older, latest);

        var handler = new GetLatestCurrencyExchangeRatesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetLatestCurrencyExchangeRatesQuery
        {
            BaseCurrencyId = ars.Id,
            QuoteCurrencyId = usd.Id,
        }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(latest.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetLatestByShortName_ReturnsCurrencyExchangeRate()
    {
        var ars = new Currency { Id = Guid.NewGuid(), Name = "Peso Argentino", ShortName = "ARS" };
        var usd = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };
        var rate = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 900m, SellRate = 910m, TimeStamp = DateTime.UtcNow };

        var currencyRepo = new Mock<IRepository<Currency, Guid>>();
        var rateRepo = new Mock<IRepository<CurrencyExchangeRate, Guid>>();

        currencyRepo.Setup(r => r.GetByAsync("ShortName", "USD", It.IsAny<CancellationToken>())).ReturnsAsync(usd);
        rateRepo.Setup(r => r.GetByAsync("QuoteCurrencyId", usd.Id, It.IsAny<CancellationToken>())).ReturnsAsync(rate);

        var handler = new GetLatestCurrencyExchangeRateByShortNameQueryHandler(_dbContext, currencyRepo.Object, rateRepo.Object);
        var result = await handler.ExecuteAsync(new GetLatestCurrencyExchangeRateByShortNameQuery { QuoteCurrencyShortName = "USD" }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(rate.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetLatestByShortName_WhenCurrencyNotFound_ReturnsFailure()
    {
        var currencyRepo = new Mock<IRepository<Currency, Guid>>();
        var rateRepo = new Mock<IRepository<CurrencyExchangeRate, Guid>>();

        currencyRepo.Setup(r => r.GetByAsync("ShortName", "XYZ", It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);

        var handler = new GetLatestCurrencyExchangeRateByShortNameQueryHandler(_dbContext, currencyRepo.Object, rateRepo.Object);
        var result = await handler.ExecuteAsync(new GetLatestCurrencyExchangeRateByShortNameQuery { QuoteCurrencyShortName = "XYZ" }, default);

        Assert.False(result.IsSuccess);
        Assert.Contains("XYZ", result.ErrorMessage);
    }

    private async Task<User> CreateCurrentUserAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "IdentityNotFound" }],
        };

        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    private async Task GrantAccessAsync(User user, params CurrencyExchangeRate[] rates)
    {
        foreach (var rate in rates)
        {
            _dbContext.CurrencyExchangeRatePermissions.Add(new CurrencyExchangeRatePermissions
            {
                ResourceId = rate.Id,
                Resource = rate,
                UserId = user.Id,
                User = user,
                PermissionLevels = [PermissionLevelEnum.Owner],
            });
        }

        await _dbContext.SaveChangesAsync();
    }
}
