using Finance.Application.Queries.CurrencyExchangeRates;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Identities;
using FinanceBackEnd.Finance.Domain.Enums;

namespace Finance.Application.Tests.Queries.CurrencyExchangeRates;

public class GetAllLatestCurrencyExchangeRatesQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task ReturnsOnlyLatestPerPair()
    {
        var user = await CreateCurrentUserAsync();
        var ars = CurrencyFixture.Build(shortName: "ARS");
        var usd = CurrencyFixture.Build(shortName: "USD");

        var older = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 800m, SellRate = 810m, TimeStamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var latest = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 900m, SellRate = 910m, TimeStamp = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc) };

        await _dbContext.CurrencyExchangeRate.AddRangeAsync(older, latest);
        await _dbContext.SaveChangesAsync();
        await GrantAccessAsync(user, older, latest);

        var handler = new GetAllLatestCurrencyExchangeRatesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllLatestCurrencyExchangeRatesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(latest.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task FiltersByCurrencyIds_MatchingBaseSide()
    {
        var user = await CreateCurrentUserAsync();
        var ars = CurrencyFixture.Build(shortName: "ARS");
        var brl = CurrencyFixture.Build(shortName: "BRL");
        var eur = CurrencyFixture.Build(shortName: "EUR");

        // arsEur: base=ars (in filter), quote=eur (not in filter) → included
        // brlEur: base=brl (not in filter), quote=eur (not in filter) → excluded
        var arsEur = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = eur, QuoteCurrencyId = eur.Id, BuyRate = 900m, SellRate = 910m, TimeStamp = DateTime.UtcNow };
        var brlEur = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = brl, BaseCurrencyId = brl.Id, QuoteCurrency = eur, QuoteCurrencyId = eur.Id, BuyRate = 5m, SellRate = 6m, TimeStamp = DateTime.UtcNow };

        await _dbContext.CurrencyExchangeRate.AddRangeAsync(arsEur, brlEur);
        await _dbContext.SaveChangesAsync();
        await GrantAccessAsync(user, arsEur, brlEur);

        var handler = new GetAllLatestCurrencyExchangeRatesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllLatestCurrencyExchangeRatesQuery(new HashSet<Guid> { ars.Id }), default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(arsEur.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task FiltersByCurrencyIds_MatchingQuoteSide()
    {
        var user = await CreateCurrentUserAsync();
        var ars = CurrencyFixture.Build(shortName: "ARS");
        var usd = CurrencyFixture.Build(shortName: "USD");
        var brl = CurrencyFixture.Build(shortName: "BRL");

        // arsUsd: base=ars, quote=usd (usd in filter set → matches via quote)
        // arsBrl: base=ars, quote=brl (neither ars nor brl in {usd.Id}) → excluded
        var arsUsd = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 900m, SellRate = 910m, TimeStamp = DateTime.UtcNow };
        var arsBrl = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = brl, QuoteCurrencyId = brl.Id, BuyRate = 5m, SellRate = 6m, TimeStamp = DateTime.UtcNow };

        await _dbContext.CurrencyExchangeRate.AddRangeAsync(arsUsd, arsBrl);
        await _dbContext.SaveChangesAsync();
        await GrantAccessAsync(user, arsUsd, arsBrl);

        var handler = new GetAllLatestCurrencyExchangeRatesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllLatestCurrencyExchangeRatesQuery(new HashSet<Guid> { usd.Id }), default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(arsUsd.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task ExcludesDeactivatedByDefault()
    {
        var user = await CreateCurrentUserAsync();
        var ars = CurrencyFixture.Build(shortName: "ARS");
        var usd = CurrencyFixture.Build(shortName: "USD");

        var active = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 900m, SellRate = 910m, TimeStamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Deactivated = false };
        var deactivated = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 950m, SellRate = 960m, TimeStamp = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc), Deactivated = true };

        await _dbContext.CurrencyExchangeRate.AddRangeAsync(active, deactivated);
        await _dbContext.SaveChangesAsync();
        await GrantAccessAsync(user, active, deactivated);

        var handler = new GetAllLatestCurrencyExchangeRatesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllLatestCurrencyExchangeRatesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(active.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task NoCurrencyIds_ReturnsAllPairs()
    {
        var user = await CreateCurrentUserAsync();
        var ars = CurrencyFixture.Build(shortName: "ARS");
        var usd = CurrencyFixture.Build(shortName: "USD");
        var brl = CurrencyFixture.Build(shortName: "BRL");

        var arsUsd = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 900m, SellRate = 910m, TimeStamp = DateTime.UtcNow };
        var arsBrl = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = ars.Id, QuoteCurrency = brl, QuoteCurrencyId = brl.Id, BuyRate = 5m, SellRate = 6m, TimeStamp = DateTime.UtcNow };

        await _dbContext.CurrencyExchangeRate.AddRangeAsync(arsUsd, arsBrl);
        await _dbContext.SaveChangesAsync();
        await GrantAccessAsync(user, arsUsd, arsBrl);

        var handler = new GetAllLatestCurrencyExchangeRatesQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllLatestCurrencyExchangeRatesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
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
