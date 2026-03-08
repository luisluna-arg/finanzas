using Finance.Application.Auth;
using Finance.Application.Commands.DebitOrigins;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.DebitOrigins;

public class CreateDebitOriginPermissionsCommandHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public CreateDebitOriginPermissionsCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task CreatePermissions_OriginAndUserFoundByClaimId_ReturnsPermissions()
    {
        const string userIdClaim = "auth0|test-user";
        var originId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = userIdClaim }],
        };
        await _dbContext.User.AddAsync(user);
        await _dbContext.DebitOrigin.AddAsync(new DebitOrigin { Id = originId, Name = "Netflix" });
        await _dbContext.SaveChangesAsync();

        var command = new CreateDebitOriginPermissionsCommand
        {
            ResourceId = originId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = userIdClaim });

        var handler = new CreateDebitOriginPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(originId, result.Data.ResourceId);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Contains(PermissionLevelEnum.Owner, result.Data.PermissionLevels);
    }

    [Fact]
    public async Task CreatePermissions_ExplicitUserId_FindsUserByIdAndReturnsPermissions()
    {
        var originId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _dbContext.User.AddAsync(new User { Id = userId, Username = "u", FirstName = "F", LastName = "L" });
        await _dbContext.DebitOrigin.AddAsync(new DebitOrigin { Id = originId, Name = "Spotify" });
        await _dbContext.SaveChangesAsync();

        var command = new CreateDebitOriginPermissionsCommand
        {
            ResourceId = originId,
            UserId = userId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext());

        var handler = new CreateDebitOriginPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(originId, result.Data.ResourceId);
        Assert.Equal(userId, result.Data.UserId);
    }

    [Fact]
    public async Task CreatePermissions_OriginNotFound_ReturnsFailure()
    {
        const string userIdClaim = "auth0|test-user";
        var userId = Guid.NewGuid();

        await _dbContext.User.AddAsync(new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = userIdClaim }],
        });
        await _dbContext.SaveChangesAsync();

        var command = new CreateDebitOriginPermissionsCommand
        {
            ResourceId = Guid.NewGuid(),
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = userIdClaim });

        var handler = new CreateDebitOriginPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
    }
}
