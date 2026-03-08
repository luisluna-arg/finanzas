using Finance.Application.Auth;
using Finance.Application.Commands;
using Finance.Application.Commands.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Funds;

public class FundOwnershipCommandHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public FundOwnershipCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task CreatePermissions_WhenFundAndUserExistByClaim_ReturnsPermissions()
    {
        const string userIdClaim = "auth0|fund-user";
        var fundId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _dbContext.User.AddAsync(new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = userIdClaim }],
        });
        await _dbContext.Fund.AddAsync(new Fund { Id = fundId, Amount = 100m, TimeStamp = DateTime.UtcNow });
        await _dbContext.SaveChangesAsync();

        var command = new CreateFundPermissionsCommand
        {
            ResourceId = fundId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = userIdClaim });

        var handler = new CreateFundPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(fundId, result.Data.ResourceId);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Contains(PermissionLevelEnum.Owner, result.Data.PermissionLevels);
    }

    [Fact]
    public async Task CreatePermissions_WhenExplicitUserIdExists_ReturnsPermissions()
    {
        var fundId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _dbContext.User.AddAsync(new User { Id = userId, Username = "u", FirstName = "F", LastName = "L" });
        await _dbContext.Fund.AddAsync(new Fund { Id = fundId, Amount = 100m, TimeStamp = DateTime.UtcNow });
        await _dbContext.SaveChangesAsync();

        var command = new CreateFundPermissionsCommand
        {
            ResourceId = fundId,
            UserId = userId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };

        var handler = new CreateFundPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Data.UserId);
    }

    [Fact]
    public async Task CreatePermissions_WhenFundDoesNotExist_ReturnsFailure()
    {
        var handler = new CreateFundPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(new CreateFundPermissionsCommand
        {
            ResourceId = Guid.NewGuid(),
            PermissionLevels = [PermissionLevelEnum.Owner],
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Resource not found", result.ErrorMessage);
    }

    [Fact]
    public async Task CreatePermissions_WhenUserDoesNotExist_ReturnsFailure()
    {
        var fundId = Guid.NewGuid();
        await _dbContext.Fund.AddAsync(new Fund { Id = fundId, Amount = 100m, TimeStamp = DateTime.UtcNow });
        await _dbContext.SaveChangesAsync();

        var command = new CreateFundPermissionsCommand
        {
            ResourceId = fundId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = "auth0|missing" });

        var handler = new CreateFundPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.ErrorMessage);
    }

    [Fact]
    public async Task DeleteOwner_WhenMatchingPermissionsExist_DeletesThem()
    {
        var fundId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "IdentityNotFound" }],
        };
        var fund = new Fund { Id = fundId, Amount = 100m, TimeStamp = DateTime.UtcNow };

        await _dbContext.User.AddAsync(user);
        await _dbContext.Fund.AddAsync(fund);
        await _dbContext.SaveChangesAsync();

        _dbContext.FundPermissions.Add(new FundPermissions
        {
            ResourceId = fundId,
            UserId = userId,
            Resource = fund,
            User = user,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await _dbContext.SaveChangesAsync();

        var command = new DeleteFundOwnerCommand { EntityId = fundId };
        command.SetContext(new FinanceDispatchContext { UserInfo = user });

        var handler = new DeleteFundOwnerCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Empty(_dbContext.FundPermissions.IgnoreQueryFilters().ToList());
    }

    [Fact]
    public async Task DeleteOwner_WhenNoPermissionsExist_ReturnsSuccess()
    {
        var command = new DeleteFundOwnerCommand { EntityId = Guid.NewGuid() };
        command.SetContext(new FinanceDispatchContext());

        var handler = new DeleteFundOwnerCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
    }
}