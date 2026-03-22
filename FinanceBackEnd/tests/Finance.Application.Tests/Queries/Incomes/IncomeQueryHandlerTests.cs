using Finance.Application.Queries.Incomes;
using Finance.Application.Repositories;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Identities;
using Finance.Domain.Models.Incomes;
using Finance.Domain.SpecialTypes;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Tests.Queries.Incomes;

public class IncomeQueryHandlerTests : QueryHandlerBaseTests
{
    [Fact]
    public async Task GetIncomes_FiltersByDateCurrencyBankAndDeactivated()
    {
        var user = await CreateCurrentUserAsync();
        var bank1 = new Bank { Id = Guid.NewGuid(), Name = "Bank 1" };
        var bank2 = new Bank { Id = Guid.NewGuid(), Name = "Bank 2" };
        var currency1 = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };
        var currency2 = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };

        var matching = new Income
        {
            Id = Guid.NewGuid(),
            Bank = bank1,
            BankId = bank1.Id,
            Currency = currency1,
            CurrencyId = currency1.Id,
            TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            Amount = new Money(100m),
            Deactivated = false,
        };

        await _dbContext.Income.AddRangeAsync(
            matching,
            new Income { Id = Guid.NewGuid(), Bank = bank1, BankId = bank1.Id, Currency = currency1, CurrencyId = currency1.Id, TimeStamp = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(10m), Deactivated = false },
            new Income { Id = Guid.NewGuid(), Bank = bank2, BankId = bank2.Id, Currency = currency1, CurrencyId = currency1.Id, TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(20m), Deactivated = false },
            new Income { Id = Guid.NewGuid(), Bank = bank1, BankId = bank1.Id, Currency = currency2, CurrencyId = currency2.Id, TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(30m), Deactivated = false },
            new Income { Id = Guid.NewGuid(), Bank = bank1, BankId = bank1.Id, Currency = currency1, CurrencyId = currency1.Id, TimeStamp = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(40m), Deactivated = true }
        );
        await _dbContext.SaveChangesAsync();

        await GrantIncomeAccessAsync(user, _dbContext.Income.IgnoreQueryFilters().ToArray());

        var repository = new Mock<IRepository<Income, Guid>>();
        repository.Setup(r => r.GetDbSet()).Returns(_dbContext.Income);

        var handler = new GetIncomesQueryHandler(_dbContext, repository.Object);

        var result = await handler.ExecuteAsync(new GetIncomesQuery
        {
            IncludeDeactivated = false,
            From = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc),
            To = new DateTime(2025, 1, 20, 0, 0, 0, DateTimeKind.Utc),
            CurrencyId = currency1.Id,
            BankId = bank1.Id,
        }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(matching.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetLatestIncome_ReturnsLatestMatchingCurrencyForBank()
    {
        var user = await CreateCurrentUserAsync();
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Bank 1" };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };

        var older = new Income { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(10m) };
        var latest = new Income { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(20m) };
        var otherCurrency = new Income { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" }, CurrencyId = Guid.NewGuid(), TimeStamp = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(30m) };

        await _dbContext.Income.AddRangeAsync(older, latest, otherCurrency);
        await _dbContext.SaveChangesAsync();

        await GrantIncomeAccessAsync(user, older, latest, otherCurrency);

        var repository = new Mock<IRepository<Income, Guid>>();
        repository.Setup(r => r.GetDbSet()).Returns(_dbContext.Income);

        var handler = new GetLatestIncomeQueryHandler(_dbContext, repository.Object);
        var result = await handler.ExecuteAsync(new GetLatestIncomeQuery(currency.Id), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(older.Id, result.Data?.Id);
    }

    [Fact]
    public async Task GetPaginatedIncomes_ReturnsOrderedPageAndTotalCount()
    {
        var user = await CreateCurrentUserAsync();
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Bank 1" };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };

        var incomes = new[]
        {
            new Income { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(1m) },
            new Income { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(2m) },
            new Income { Id = Guid.NewGuid(), Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc), Amount = new Money(3m) },
        };

        await _dbContext.Income.AddRangeAsync(incomes);
        await _dbContext.SaveChangesAsync();

        await GrantIncomeAccessAsync(user, incomes);

        var handler = new GetPaginatedIncomesQueryHandler(_dbContext);

        var result = await handler.ExecuteAsync(new GetPaginatedIncomesQuery
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
    public async Task GetSingleIncome_ReturnsRepositoryResult()
    {
        var income = new Income { Id = Guid.NewGuid(), Amount = new Money(100m), TimeStamp = DateTime.UtcNow };
        var repository = new Mock<IRepository<Income, Guid>>();
        repository.Setup(r => r.GetByIdAsync(income.Id, It.IsAny<CancellationToken>())).ReturnsAsync(income);

        var handler = new GetSingleIncomeQueryHandler(_dbContext, repository.Object);
        var result = await handler.ExecuteAsync(new GetSingleIncomeQuery { Id = income.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(income, result.Data);
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

    private async Task GrantIncomeAccessAsync(User user, params Income[] incomes)
    {
        foreach (var income in incomes)
        {
            _dbContext.IncomePermissions.Add(new IncomePermissions
            {
                ResourceId = income.Id,
                Resource = income,
                UserId = user.Id,
                User = user,
                PermissionLevels = [FinanceBackEnd.Finance.Domain.Enums.PermissionLevelEnum.Owner],
            });
        }

        await _dbContext.SaveChangesAsync();
    }
}
