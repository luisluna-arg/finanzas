using Finance.Application.Queries.Debits;
using Finance.Domain.Enums;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Queries.Debits;

public class DebitQueryHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public DebitQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    private async Task SeedDebitsAsync(params Debit[] debits)
    {
        await _dbContext.Debit.AddRangeAsync(debits);
        await _dbContext.SaveChangesAsync();
    }

    private static DebitOrigin MakeOrigin(Guid? appModuleId = null, AppModuleType? type = null)
    {
        type ??= new AppModuleType { Id = AppModuleTypeEnum.Debits };
        var appModule = new AppModule { Id = appModuleId ?? Guid.NewGuid(), Name = "TestModule", Type = type };
        return new DebitOrigin { Id = Guid.NewGuid(), Name = "TestOrigin", AppModule = appModule, AppModuleId = appModule.Id };
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

    private async Task GrantDebitAccessAsync(User user, params Debit[] debits)
    {
        var originIds = new HashSet<Guid>();

        foreach (var debit in debits)
        {
            _dbContext.DebitPermissions.Add(new DebitPermissions
            {
                ResourceId = debit.Id,
                Resource = debit,
                UserId = user.Id,
                User = user,
                PermissionLevels = [PermissionLevelEnum.Owner],
            });

            if (debit.Origin != null && originIds.Add(debit.Origin.Id))
            {
                _dbContext.DebitOriginPermissions.Add(new DebitOriginPermissions
                {
                    ResourceId = debit.Origin.Id,
                    Resource = debit.Origin,
                    UserId = user.Id,
                    User = user,
                    PermissionLevels = [PermissionLevelEnum.Owner],
                });
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    #region GetAllDebits

    [Fact]
    public async Task GetAll_ReturnsAllActiveDebits_WhenIncludeDeactivatedFalse()
    {
        var user = await CreateCurrentUserAsync();
        var origin = MakeOrigin();
        var active = new Debit { Id = Guid.NewGuid(), Origin = origin, OriginId = origin.Id, Amount = 100m, Deactivated = false, TimeStamp = DateTime.UtcNow };
        var inactive = new Debit { Id = Guid.NewGuid(), Origin = origin, OriginId = origin.Id, Amount = 200m, Deactivated = true, TimeStamp = DateTime.UtcNow };

        await SeedDebitsAsync(active, inactive);
        await GrantDebitAccessAsync(user, active, inactive);

        var handler = new GetAllDebitsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllDebitsQuery { IncludeDeactivated = false }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(active.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetAll_ReturnsAllDebits_WhenIncludeDeactivatedTrue()
    {
        var user = await CreateCurrentUserAsync();
        var origin = MakeOrigin();
        var active = new Debit { Id = Guid.NewGuid(), Origin = origin, OriginId = origin.Id, Amount = 100m, Deactivated = false, TimeStamp = DateTime.UtcNow };
        var inactive = new Debit { Id = Guid.NewGuid(), Origin = origin, OriginId = origin.Id, Amount = 200m, Deactivated = true, TimeStamp = DateTime.UtcNow };

        await SeedDebitsAsync(active, inactive);
        await GrantDebitAccessAsync(user, active, inactive);

        var handler = new GetAllDebitsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllDebitsQuery { IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task GetAll_FiltersbyAppModuleId()
    {
        var user = await CreateCurrentUserAsync();
        var appModuleId = Guid.NewGuid();
        var sharedType = new AppModuleType { Id = AppModuleTypeEnum.Debits };
        var matchingOrigin = MakeOrigin(appModuleId, sharedType);
        var otherOrigin = MakeOrigin(null, sharedType);

        var matching = new Debit { Id = Guid.NewGuid(), Origin = matchingOrigin, OriginId = matchingOrigin.Id, Amount = 100m, TimeStamp = DateTime.UtcNow };
        var other = new Debit { Id = Guid.NewGuid(), Origin = otherOrigin, OriginId = otherOrigin.Id, Amount = 200m, TimeStamp = DateTime.UtcNow };

        await SeedDebitsAsync(matching, other);
        await GrantDebitAccessAsync(user, matching, other);

        var handler = new GetAllDebitsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllDebitsQuery { AppModuleId = appModuleId, IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(matching.Id, result.Data[0].Id);
    }

    #endregion

    #region GetPaginatedDebits

    [Fact]
    public async Task GetPaginated_ReturnsPagedResults()
    {
        var user = await CreateCurrentUserAsync();
        var origin = MakeOrigin();
        var debits = Enumerable.Range(1, 5).Select(i => new Debit
        {
            Id = Guid.NewGuid(),
            Origin = origin,
            OriginId = origin.Id,
            Amount = (decimal)i * 10,
            TimeStamp = DateTime.UtcNow.AddDays(-i),
        }).ToArray();

        await SeedDebitsAsync(debits);
        await GrantDebitAccessAsync(user, debits);

        var handler = new GetPaginatedDebitsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetPaginatedDebitsQuery { Page = 1, PageSize = 2, IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Items.Count());
        Assert.Equal(5, result.Data.TotalItems);
    }

    [Fact]
    public async Task GetPaginated_FiltersByAppModuleId()
    {
        var user = await CreateCurrentUserAsync();
        var appModuleId = Guid.NewGuid();
        var sharedType = new AppModuleType { Id = AppModuleTypeEnum.Debits };
        var matchingOrigin = MakeOrigin(appModuleId, sharedType);
        var otherOrigin = MakeOrigin(null, sharedType);

        var matching = new Debit { Id = Guid.NewGuid(), Origin = matchingOrigin, OriginId = matchingOrigin.Id, Amount = 100m, TimeStamp = DateTime.UtcNow };
        var other = new Debit { Id = Guid.NewGuid(), Origin = otherOrigin, OriginId = otherOrigin.Id, Amount = 200m, TimeStamp = DateTime.UtcNow };

        await SeedDebitsAsync(matching, other);
        await GrantDebitAccessAsync(user, matching, other);

        var handler = new GetPaginatedDebitsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetPaginatedDebitsQuery { AppModuleId = appModuleId, Page = 1, PageSize = 10, IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data.Items);
    }

    #endregion

    #region GetLatestDebits

    [Fact]
    public async Task GetLatest_ReturnsLatestDebitPerOrigin()
    {
        var user = await CreateCurrentUserAsync();
        var appModuleId = Guid.NewGuid();
        var origin = MakeOrigin(appModuleId);

        var older = new Debit { Id = Guid.NewGuid(), Origin = origin, OriginId = origin.Id, Amount = 50m, TimeStamp = DateTime.UtcNow.AddDays(-5) };
        var newer = new Debit { Id = Guid.NewGuid(), Origin = origin, OriginId = origin.Id, Amount = 100m, TimeStamp = DateTime.UtcNow };

        await SeedDebitsAsync(older, newer);
        await GrantDebitAccessAsync(user, older, newer);

        var handler = new GetLatestDebitsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetLatestDebitsQuery { AppModuleId = appModuleId, IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(newer.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetLatest_ExcludesDeactivated_WhenIncludeDeactivatedFalse()
    {
        var user = await CreateCurrentUserAsync();
        var appModuleId = Guid.NewGuid();
        var origin = MakeOrigin(appModuleId);

        var deactivated = new Debit { Id = Guid.NewGuid(), Origin = origin, OriginId = origin.Id, Amount = 100m, TimeStamp = DateTime.UtcNow, Deactivated = true };

        await SeedDebitsAsync(deactivated);
        await GrantDebitAccessAsync(user, deactivated);

        var handler = new GetLatestDebitsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetLatestDebitsQuery { AppModuleId = appModuleId, IncludeDeactivated = false }, default);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }

    #endregion
}
