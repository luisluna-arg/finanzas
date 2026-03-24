using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Queries.Summary;
using Finance.Application.Services;
using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Identities;
using Finance.Domain.Models.IOLInvestments;
using Finance.Domain.Policies;
using Finance.Domain.SpecialTypes;
using Finance.Persistence.Constants;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Queries.Summary;

public class GetCurrentInvestmentsQueryHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly IMemoryCache _cache;
    private readonly CurrencyConversionService _currencyConverter;

    public GetCurrentInvestmentsQueryHandlerTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _currencyConverter = new CurrencyConversionService(_dispatcher.Object, _dbContext, new CurrencyConversionPolicy(), _cache);
    }

    public override void Dispose()
    {
        base.Dispose();
        _cache.Dispose();
    }

    private GetCurrentInvestmentsQueryHandler CreateHandler() =>
        new(_dbContext, _currencyConverter);

    private void SetupDispatcherRates(List<CurrencyExchangeRate> rates)
    {
        _dispatcher
            .Setup(d => d.DispatchQueryAsync<List<CurrencyExchangeRate>>(It.IsAny<IQuery<List<CurrencyExchangeRate>>>()))
            .ReturnsAsync(DataResult<List<CurrencyExchangeRate>>.Success(rates));
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

    private async Task GrantInvestmentAccessAsync(User user, params IOLInvestment[] investments)
    {
        foreach (var inv in investments)
        {
            _dbContext.IOLInvestmentPermissions.Add(new IOLInvestmentPermissions
            {
                ResourceId = inv.Id,
                Resource = inv,
                UserId = user.Id,
                User = user,
                PermissionLevels = [PermissionLevelEnum.Owner],
            });
            _dbContext.IOLInvestmentAssetPermissions.Add(new IOLInvestmentAssetPermissions
            {
                ResourceId = inv.Asset.Id,
                Resource = inv.Asset,
                UserId = user.Id,
                User = user,
                PermissionLevels = [PermissionLevelEnum.Owner],
            });
        }
        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task ReturnsEmptyResult_WhenNoInvestments()
    {
        await CreateCurrentUserAsync();

        var result = await CreateHandler().ExecuteAsync(new GetCurrentInvestmentsQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data.Items);
    }

    [Fact]
    public async Task ValuedDefaultCurrency_EqualsValued_WhenAssetIsInDefaultCurrency()
    {
        var user = await CreateCurrentUserAsync();
        var defaultCurrencyId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);
        var currency = (await _dbContext.Currency.FindAsync(defaultCurrencyId))!;
        var asset = new IOLInvestmentAsset { Id = Guid.NewGuid(), Symbol = "AAPL", TypeId = IOLInvestmentAssetTypeEnum.Cedear, CurrencyId = defaultCurrencyId, Currency = currency };
        var investment = new IOLInvestment { Id = Guid.NewGuid(), Asset = asset, AssetId = asset.Id, TimeStamp = DateTime.UtcNow, Valued = new Money(100m) };

        await _dbContext.IOLInvestmentAsset.AddAsync(asset);
        await _dbContext.IOLInvestment.AddAsync(investment);
        await _dbContext.SaveChangesAsync();
        await GrantInvestmentAccessAsync(user, investment);

        SetupDispatcherRates([]);  // same-currency: no rate lookup needed but service always fetches

        var result = await CreateHandler().ExecuteAsync(new GetCurrentInvestmentsQuery(), default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data.Items);
        Assert.Equal(100m, (decimal)item.Valued);
        Assert.Equal(100m, (decimal)item.ValuedDefaultCurrency);
    }

    [Fact]
    public async Task ValuedDefaultCurrency_IsConverted_WhenAssetIsInForeignCurrency()
    {
        var user = await CreateCurrentUserAsync();
        var defaultCurrencyId = Guid.Parse(CurrencyConstants.DefaultCurrencyId); // ARS

        var usd = CurrencyFixture.Build();
        var rate = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrencyId = defaultCurrencyId, QuoteCurrencyId = usd.Id, BuyRate = 1000m, SellRate = 1100m, TimeStamp = DateTime.UtcNow };

        var asset = new IOLInvestmentAsset { Id = Guid.NewGuid(), Symbol = "GLD", TypeId = IOLInvestmentAssetTypeEnum.Cedear, CurrencyId = usd.Id, Currency = usd };
        var investment = new IOLInvestment { Id = Guid.NewGuid(), Asset = asset, AssetId = asset.Id, TimeStamp = DateTime.UtcNow, Valued = new Money(5m) };

        await _dbContext.Currency.AddAsync(usd);
        await _dbContext.IOLInvestmentAsset.AddAsync(asset);
        await _dbContext.IOLInvestment.AddAsync(investment);
        await _dbContext.SaveChangesAsync();
        await GrantInvestmentAccessAsync(user, investment);

        SetupDispatcherRates([rate]);

        var result = await CreateHandler().ExecuteAsync(new GetCurrentInvestmentsQuery(), default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data.Items);
        Assert.Equal(5m, (decimal)item.Valued);
        Assert.Equal(5500m, (decimal)item.ValuedDefaultCurrency); // 5 USD * sellRate 1100 = 5500 ARS
    }

    [Fact]
    public async Task MultipleInvestments_EachGetsCorrectConvertedValue()
    {
        var user = await CreateCurrentUserAsync();
        var arsId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);
        var ars = (await _dbContext.Currency.FindAsync(arsId))!;
        var usd = CurrencyFixture.Build();
        var rate = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrencyId = arsId, QuoteCurrencyId = usd.Id, BuyRate = 1000m, SellRate = 1100m, TimeStamp = DateTime.UtcNow };

        var ts = DateTime.UtcNow;
        var assetArs = new IOLInvestmentAsset { Id = Guid.NewGuid(), Symbol = "AA", TypeId = IOLInvestmentAssetTypeEnum.Cedear, CurrencyId = arsId, Currency = ars };
        var assetUsd = new IOLInvestmentAsset { Id = Guid.NewGuid(), Symbol = "BB", TypeId = IOLInvestmentAssetTypeEnum.Cedear, CurrencyId = usd.Id, Currency = usd };
        var inv1 = new IOLInvestment { Id = Guid.NewGuid(), Asset = assetArs, AssetId = assetArs.Id, TimeStamp = ts, Valued = new Money(3000m) };
        var inv2 = new IOLInvestment { Id = Guid.NewGuid(), Asset = assetUsd, AssetId = assetUsd.Id, TimeStamp = ts, Valued = new Money(5m) };

        await _dbContext.Currency.AddAsync(usd);
        await _dbContext.IOLInvestmentAsset.AddRangeAsync(assetArs, assetUsd);
        await _dbContext.IOLInvestment.AddRangeAsync(inv1, inv2);
        await _dbContext.SaveChangesAsync();
        await GrantInvestmentAccessAsync(user, inv1, inv2);

        SetupDispatcherRates([rate]);

        var result = await CreateHandler().ExecuteAsync(new GetCurrentInvestmentsQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Items.Count);

        var aa = result.Data.Items.Single(i => i.Label == "AA");
        var bb = result.Data.Items.Single(i => i.Label == "BB");

        Assert.Equal(3000m, (decimal)aa.ValuedDefaultCurrency);  // ARS→ARS: passthrough
        Assert.Equal(5500m, (decimal)bb.ValuedDefaultCurrency);  // 5 USD * 1100 = 5500 ARS
    }
}
