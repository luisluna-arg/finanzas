using Finance.Application.Auth;
using Finance.Application.Commands.Debits;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Debits;

public class CreateDebitPermissionsCommandHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public CreateDebitPermissionsCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task CreatePermissions_DebitAndUserFoundByClaimId_ReturnsPermissions()
    {
        const string userIdClaim = "auth0|test-user";
        var debitId = Guid.NewGuid();
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
        await _dbContext.Debit.AddAsync(new Debit { Id = debitId });
        await _dbContext.SaveChangesAsync();

        var command = new CreateDebitPermissionsCommand
        {
            ResourceId = debitId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = userIdClaim });

        var handler = new CreateDebitPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(debitId, result.Data.ResourceId);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Contains(PermissionLevelEnum.Owner, result.Data.PermissionLevels);
    }

    [Fact]
    public async Task CreatePermissions_ExplicitUserId_FindsUserByIdAndReturnsPermissions()
    {
        var debitId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _dbContext.User.AddAsync(new User { Id = userId, Username = "u", FirstName = "F", LastName = "L" });
        await _dbContext.Debit.AddAsync(new Debit { Id = debitId });
        await _dbContext.SaveChangesAsync();

        var command = new CreateDebitPermissionsCommand
        {
            ResourceId = debitId,
            UserId = userId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };

        var handler = new CreateDebitPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Data.UserId);
    }

    [Fact]
    public async Task CreatePermissions_DebitNotFound_ReturnsFailure()
    {
        var command = new CreateDebitPermissionsCommand
        {
            ResourceId = Guid.NewGuid(),
            PermissionLevels = [PermissionLevelEnum.Owner],
        };

        var handler = new CreateDebitPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Resource not found", result.ErrorMessage);
    }

    [Fact]
    public async Task CreatePermissions_UserNotFound_ReturnsFailure()
    {
        var debitId = Guid.NewGuid();
        await _dbContext.Debit.AddAsync(new Debit { Id = debitId });
        await _dbContext.SaveChangesAsync();

        var command = new CreateDebitPermissionsCommand
        {
            ResourceId = debitId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = "auth0|unknown" });

        var handler = new CreateDebitPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
    }
}
