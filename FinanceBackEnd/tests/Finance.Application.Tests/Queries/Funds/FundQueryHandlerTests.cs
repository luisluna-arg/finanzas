using Finance.Application.Legacy.Repositories;
using Finance.Application.Queries.Funds;
using Finance.Application.Queries.Movements;
using Finance.Application.Repositories;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Identities;
using Finance.Domain.Models.Movements;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Queries.Funds;

public class FundQueryHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public FundQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task GetFunds_FiltersByDateCurrencyBankDailyUseAndDeactivated()
    {
        var user = await CreateCurrentUserAsync();
        var bank1 = new Bank { Id = Guid.NewGuid(), Name = "Bank 1" };
        var bank2 = new Bank { Id = Guid.NewGuid(), Name = "Bank 2" };
        var currency1 = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };
        var currency2 = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };

        var matching = new Fund
        {
            Id = Guid.NewGuid(),
            Bank = bank1,
            BankId = bank1.Id,
            Currency = currency1,
            CurrencyId = currency1.Id,
            TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            Amount = 100m,
            DailyUse = true,
            Deactivated = false,
        };

        await _dbContext.Fund.AddRangeAsync(
            matching,
            new Fund { Id = Guid.NewGuid(), Bank = bank1, BankId = bank1.Id, Currency = currency1, CurrencyId = currency1.Id, TimeStamp = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc), CreatedAt = DateTime.UtcNow, Amount = 10m, DailyUse = false, Deactivated = false },
            new Fund { Id = Guid.NewGuid(), Bank = bank2, BankId = bank2.Id, Currency = currency1, CurrencyId = currency1.Id, TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), CreatedAt = DateTime.UtcNow, Amount = 20m, DailyUse = true, Deactivated = false },
            new Fund { Id = Guid.NewGuid(), Bank = bank1, BankId = bank1.Id, Currency = currency2, CurrencyId = currency2.Id, TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), CreatedAt = DateTime.UtcNow, Amount = 30m, DailyUse = true, Deactivated = false },
            new Fund { Id = Guid.NewGuid(), Bank = bank1, BankId = bank1.Id, Currency = currency1, CurrencyId = currency1.Id, TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), CreatedAt = DateTime.UtcNow, Amount = 40m, DailyUse = true, Deactivated = true }
        );
        await _dbContext.SaveChangesAsync();

        await GrantFundAccessAsync(user, _dbContext.Fund.IgnoreQueryFilters().ToArray());

        var repository = new Mock<IRepository<Fund, Guid>>();
        repository.Setup(r => r.GetDbSet()).Returns(_dbContext.Fund);

        var handler = new GetFundsQueryHandler(_dbContext, repository.Object);

        var result = await handler.ExecuteAsync(new GetFundsQuery
        {
            IncludeDeactivated = false,
            From = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc),
            To = new DateTime(2025, 1, 20, 0, 0, 0, DateTimeKind.Utc),
            CurrencyId = currency1.Id,
            BankId = bank1.Id,
            DailyUse = true,
        }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(matching.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetLatestFund_ReturnsLatestFundForBank()
    {
        var user = await CreateCurrentUserAsync();
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Bank 1" };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };
        var older = new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Amount = 10m, DailyUse = true };
        var latest = new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc), Amount = 20m, DailyUse = true };
        var otherBank = new Fund { Id = Guid.NewGuid(), Bank = new Bank { Id = Guid.NewGuid(), Name = "Bank 2" }, BankId = Guid.NewGuid(), Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc), Amount = 30m, DailyUse = true };

        await _dbContext.Fund.AddRangeAsync(older, latest, otherBank);
        await _dbContext.SaveChangesAsync();

        await GrantFundAccessAsync(user, older, latest, otherBank);

        var repository = new Mock<IRepository<Fund, Guid>>();
        repository.Setup(r => r.GetDbSet()).Returns(_dbContext.Fund);

        var handler = new GetLatestFundQueryHandler(_dbContext, repository.Object);

        var result = await handler.ExecuteAsync(new GetLatestFundQuery(bank.Id) { DailyUse = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(latest.Id, result.Data?.Id);
    }

    [Fact]
    public async Task GetPaginatedFunds_ReturnsOrderedPageAndTotalCount()
    {
        var user = await CreateCurrentUserAsync();
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Bank 1" };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };

        var funds = new[]
        {
            new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Amount = 1m },
            new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc), Amount = 2m },
            new Fund { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc), Amount = 3m }
        };

        await _dbContext.Fund.AddRangeAsync(funds);
        await _dbContext.SaveChangesAsync();

        await GrantFundAccessAsync(user, funds);

        var handler = new GetPaginatedFundsQueryHandler(_dbContext);

        var result = await handler.ExecuteAsync(new GetPaginatedFundsQuery
        {
            Page = 2,
            PageSize = 1,
            IncludeDeactivated = true,
        }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Data.TotalItems);
        var items = result.Data.Items.ToList();
        Assert.Single(items);
        Assert.Equal(2m, (decimal)items[0].Amount);
    }

    [Fact]
    public async Task GetSingleFund_ReturnsRepositoryResult()
    {
        var fund = new Fund { Id = Guid.NewGuid(), Amount = 100m, TimeStamp = DateTime.UtcNow };
        var repository = new Mock<IRepository<Fund, Guid>>();
        repository.Setup(r => r.GetByIdAsync(fund.Id, It.IsAny<CancellationToken>())).ReturnsAsync(fund);

        var handler = new GetSingleFundQueryHandler(_dbContext, repository.Object);

        var result = await handler.ExecuteAsync(new GetSingleFundQuery { Id = fund.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(fund, result.Data);
    }

    [Fact]
    public async Task GetFundMovements_FiltersByFundsModuleDatesAndDeactivated()
    {
        var user = await CreateCurrentUserAsync();
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Bank 1" };
        var fundModule = new AppModule { Id = Guid.NewGuid(), Name = "Funds", Currency = currency };
        var otherModule = new AppModule { Id = Guid.NewGuid(), Name = "Other", Currency = currency };

        var matching = new Movement
        {
            Id = Guid.NewGuid(),
            AppModule = fundModule,
            AppModuleId = fundModule.Id,
            Bank = bank,
            BankId = bank.Id,
            Currency = currency,
            CurrencyId = currency.Id,
            TimeStamp = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            Concept1 = "ok",
            Amount = 100m,
            Deactivated = false,
        };

        await _dbContext.Movement.AddRangeAsync(
            matching,
            new Movement { Id = Guid.NewGuid(), AppModule = fundModule, AppModuleId = fundModule.Id, Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc), CreatedAt = DateTime.UtcNow, Concept1 = "out-of-range", Amount = 200m, Deactivated = false },
            new Movement { Id = Guid.NewGuid(), AppModule = fundModule, AppModuleId = fundModule.Id, Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc), CreatedAt = DateTime.UtcNow, Concept1 = "deactivated", Amount = 300m, Deactivated = true },
            new Movement { Id = Guid.NewGuid(), AppModule = otherModule, AppModuleId = otherModule.Id, Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc), CreatedAt = DateTime.UtcNow, Concept1 = "other-module", Amount = 400m, Deactivated = false }
        );
        await _dbContext.SaveChangesAsync();

        await GrantMovementAccessAsync(user, _dbContext.Movement.IgnoreQueryFilters().ToList());

        var appModuleRepository = new Mock<IAppModuleRepository>();
        appModuleRepository.Setup(r => r.GetFundsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(fundModule);
        var movementRepository = new Mock<IRepository<Movement, Guid>>();
        movementRepository.Setup(r => r.GetDbSet()).Returns(_dbContext.Movement);

        var handler = new GetFundMovementsQueryHandler(_dbContext, appModuleRepository.Object, movementRepository.Object);

        var result = await handler.ExecuteAsync(new GetFundMovementsQuery
        {
            IncludeDeactivated = false,
            From = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            To = new DateTime(2025, 1, 31, 0, 0, 0, DateTimeKind.Utc),
        }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(matching.Id, result.Data[0].Id);
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
                PermissionLevels = [FinanceBackEnd.Finance.Domain.Enums.PermissionLevelEnum.Owner],
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task GrantMovementAccessAsync(User user, IEnumerable<Movement> movements)
    {
        foreach (var movement in movements)
        {
            _dbContext.MovementPermissions.Add(new MovementPermissions
            {
                ResourceId = movement.Id,
                Resource = movement,
                UserId = user.Id,
                User = user,
                PermissionLevels = [FinanceBackEnd.Finance.Domain.Enums.PermissionLevelEnum.Owner],
            });
        }

        await _dbContext.SaveChangesAsync();
    }
}