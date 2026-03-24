using Finance.Application.Queries.Summary;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Identities;
using Finance.Domain.Policies;
using Finance.Persistence.Constants;
using FinanceBackEnd.Finance.Domain.Enums;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Queries.Summary;

public class GetCurrentFundsQueryHandlerTests : QueryHandlerBaseTests
{

    private GetCurrentFundsQueryHandler CreateHandler() =>
        new(_dbContext, new CurrencyConversionPolicy());

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

    private async Task GrantFundAccessAsync(User user, params Fund[] funds)
    {
        foreach (var fund in funds)
        {
            _dbContext.FundPermissions.Add(new FundPermissions
            {
                ResourceId = fund.Id,
                Resource = fund,
                UserId = user.Id,
                User = user,
                PermissionLevels = [PermissionLevelEnum.Owner],
            });
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task GrantRateAccessAsync(User user, params CurrencyExchangeRate[] rates)
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

    [Fact]
    public async Task ReturnsSuccess_WhenNoFunds()
    {
        var result = await CreateHandler().ExecuteAsync(new GetCurrentFundsQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data.Items);
    }

    [Fact]
    public async Task FundInDefaultCurrency_HasQuoteCurrencyValueEqualToAmount()
    {
        var user = await CreateCurrentUserAsync();
        var arsId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);
        var ars = (await _dbContext.Currency.FindAsync(arsId))!;
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Banco Nacion" };
        var fund = new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = ars, CurrencyId = arsId, Amount = 5000m, TimeStamp = DateTime.UtcNow, DailyUse = true };

        await _dbContext.Bank.AddAsync(bank);
        await _dbContext.Fund.AddAsync(fund);
        await _dbContext.SaveChangesAsync();
        await GrantFundAccessAsync(user, fund);

        var result = await CreateHandler().ExecuteAsync(new GetCurrentFundsQuery { CurrencyId = arsId }, default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data.Items);
        Assert.Equal(5000m, (decimal)item.Value);
        Assert.Equal(5000m, (decimal)item.QuoteCurrencyValue);
    }

    [Fact]
    public async Task FundInForeignCurrency_IsConvertedToDefaultCurrency()
    {
        var user = await CreateCurrentUserAsync();
        var arsId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);
        var ars = (await _dbContext.Currency.FindAsync(arsId))!;
        var usd = CurrencyFixture.Build();

        // base=ARS, quote=USD: 1 USD = 1100 ARS (sellRate)
        // fund holds USD → convert to ARS: source is quote → amount * sellRate
        var rate = new CurrencyExchangeRate { Id = Guid.NewGuid(), BaseCurrency = ars, BaseCurrencyId = arsId, QuoteCurrency = usd, QuoteCurrencyId = usd.Id, BuyRate = 1000m, SellRate = 1100m, TimeStamp = DateTime.UtcNow };

        var bank = new Bank { Id = Guid.NewGuid(), Name = "Brubank" };
        var fund = new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = usd, CurrencyId = usd.Id, Amount = 5m, TimeStamp = DateTime.UtcNow };

        await _dbContext.Currency.AddAsync(usd);
        await _dbContext.CurrencyExchangeRate.AddAsync(rate);
        await _dbContext.Bank.AddAsync(bank);
        await _dbContext.Fund.AddAsync(fund);
        await _dbContext.SaveChangesAsync();
        await GrantFundAccessAsync(user, fund);
        await GrantRateAccessAsync(user, rate);

        var result = await CreateHandler().ExecuteAsync(new GetCurrentFundsQuery { CurrencyId = arsId }, default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data.Items);
        Assert.Equal(5m, (decimal)item.Value);
        Assert.Equal(5500m, (decimal)item.QuoteCurrencyValue); // 5 * 1100
    }

    [Fact]
    public async Task FundInForeignCurrency_IsSkipped_WhenNoRateFound()
    {
        var user = await CreateCurrentUserAsync();
        var arsId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);
        var usd = CurrencyFixture.Build();

        var bank = new Bank { Id = Guid.NewGuid(), Name = "Brubank" };
        var fund = new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = usd, CurrencyId = usd.Id, Amount = 10m, TimeStamp = DateTime.UtcNow };

        await _dbContext.Currency.AddAsync(usd);
        await _dbContext.Bank.AddAsync(bank);
        await _dbContext.Fund.AddAsync(fund);
        await _dbContext.SaveChangesAsync();
        await GrantFundAccessAsync(user, fund);

        var result = await CreateHandler().ExecuteAsync(new GetCurrentFundsQuery { CurrencyId = arsId }, default);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data.Items);
    }

    [Fact]
    public async Task OnlyLatestFund_PerBankAndCurrencyPair_IsUsed()
    {
        var user = await CreateCurrentUserAsync();
        var arsId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);
        var ars = (await _dbContext.Currency.FindAsync(arsId))!;
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Galicia" };

        var older = new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = ars, CurrencyId = arsId, Amount = 100m, TimeStamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var latest = new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = ars, CurrencyId = arsId, Amount = 999m, TimeStamp = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc) };

        await _dbContext.Bank.AddAsync(bank);
        await _dbContext.Fund.AddRangeAsync(older, latest);
        await _dbContext.SaveChangesAsync();
        await GrantFundAccessAsync(user, older, latest);

        var result = await CreateHandler().ExecuteAsync(new GetCurrentFundsQuery { CurrencyId = arsId }, default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Data.Items);
        Assert.Equal(999m, (decimal)item.Value);
    }
}
