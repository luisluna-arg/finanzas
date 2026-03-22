using Finance.Application.Queries.DebitOrigins;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Identities;
using FinanceBackEnd.Finance.Domain.Enums;

namespace Finance.Application.Tests.Queries.DebitOrigins;

public class DebitOriginQueryHandlerTests : QueryHandlerBaseTests
{
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

    private async Task GrantOriginAccessAsync(User user, params DebitOrigin[] origins)
    {
        foreach (var origin in origins)
        {
            _dbContext.DebitOriginPermissions.Add(new DebitOriginPermissions
            {
                ResourceId = origin.Id,
                Resource = origin,
                UserId = user.Id,
                User = user,
                PermissionLevels = [PermissionLevelEnum.Owner],
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    private DebitOrigin MakeOrigin(string name = "Netflix", bool deactivated = false)
    {
        var appModule = new AppModule { Id = Guid.NewGuid(), Name = "Personal" };
        return new DebitOrigin { Id = Guid.NewGuid(), Name = name, AppModule = appModule, AppModuleId = appModule.Id, Deactivated = deactivated };
    }

    #region GetAllDebitOrigins

    [Fact]
    public async Task GetAll_ActiveOnly_ReturnsOnlyActiveOrigins()
    {
        var user = await CreateCurrentUserAsync();
        var active = MakeOrigin("Netflix", deactivated: false);
        var inactive = MakeOrigin("Spotify", deactivated: true);

        _dbContext.DebitOrigin.AddRange(active, inactive);
        await _dbContext.SaveChangesAsync();
        await GrantOriginAccessAsync(user, active, inactive);

        var handler = new GetAllDebitOriginsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllDebitOriginsQuery { IncludeDeactivated = false }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(active.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetAll_IncludeDeactivated_ReturnsAllOrigins()
    {
        var user = await CreateCurrentUserAsync();
        var active = MakeOrigin("Netflix", deactivated: false);
        var inactive = MakeOrigin("Spotify", deactivated: true);

        _dbContext.DebitOrigin.AddRange(active, inactive);
        await _dbContext.SaveChangesAsync();
        await GrantOriginAccessAsync(user, active, inactive);

        var handler = new GetAllDebitOriginsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllDebitOriginsQuery { IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task GetAll_FiltersToCurrentUser_DoesNotReturnUnownedOrigins()
    {
        var user = await CreateCurrentUserAsync();
        var owned = MakeOrigin("Netflix");
        var unowned = MakeOrigin("Hulu");

        _dbContext.DebitOrigin.AddRange(owned, unowned);
        await _dbContext.SaveChangesAsync();
        await GrantOriginAccessAsync(user, owned); // only grant access to 'owned'

        var handler = new GetAllDebitOriginsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllDebitOriginsQuery { IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal(owned.Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetAll_OrdersByNameThenAppModuleName()
    {
        var user = await CreateCurrentUserAsync();
        var moduleA = new AppModule { Id = Guid.NewGuid(), Name = "Alpha" };
        var moduleB = new AppModule { Id = Guid.NewGuid(), Name = "Beta" };

        var originZ = new DebitOrigin { Id = Guid.NewGuid(), Name = "Zzz", AppModule = moduleA, AppModuleId = moduleA.Id };
        var originA1 = new DebitOrigin { Id = Guid.NewGuid(), Name = "Aaa", AppModule = moduleB, AppModuleId = moduleB.Id };
        var originA2 = new DebitOrigin { Id = Guid.NewGuid(), Name = "Aaa", AppModule = moduleA, AppModuleId = moduleA.Id };

        _dbContext.DebitOrigin.AddRange(originZ, originA1, originA2);
        await _dbContext.SaveChangesAsync();
        await GrantOriginAccessAsync(user, originZ, originA1, originA2);

        var handler = new GetAllDebitOriginsQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetAllDebitOriginsQuery { IncludeDeactivated = true }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(originA2.Id, result.Data[0].Id); // Aaa / Alpha first
        Assert.Equal(originA1.Id, result.Data[1].Id); // Aaa / Beta second
        Assert.Equal(originZ.Id, result.Data[2].Id);  // Zzz last
    }

    #endregion

    #region GetDebitOrigin

    [Fact]
    public async Task GetById_ExistingOwned_ReturnsOrigin()
    {
        var user = await CreateCurrentUserAsync();
        var origin = MakeOrigin("Netflix");

        _dbContext.DebitOrigin.Add(origin);
        await _dbContext.SaveChangesAsync();
        await GrantOriginAccessAsync(user, origin);

        var handler = new GetDebitOriginQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetDebitOriginQuery { Id = origin.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(origin.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetById_Unowned_ReturnsNull()
    {
        var user = await CreateCurrentUserAsync();
        var origin = MakeOrigin("Netflix");

        _dbContext.DebitOrigin.Add(origin);
        await _dbContext.SaveChangesAsync();
        // no grant

        var handler = new GetDebitOriginQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetDebitOriginQuery { Id = origin.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNull()
    {
        await CreateCurrentUserAsync();

        var handler = new GetDebitOriginQueryHandler(_dbContext);
        var result = await handler.ExecuteAsync(new GetDebitOriginQuery { Id = Guid.NewGuid() }, default);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Data);
    }

    #endregion
}
