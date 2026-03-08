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

public class DeleteDebitOriginOwnerCommandHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public DeleteDebitOriginOwnerCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task DeleteOwner_MatchingPermissionsExist_DeletesAndReturnsSuccess()
    {
        var originId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "some-claim" }],
        };
        var origin = new DebitOrigin { Id = originId, Name = "Netflix" };
        await _dbContext.User.AddAsync(user);
        await _dbContext.DebitOrigin.AddAsync(origin);
        await _dbContext.SaveChangesAsync();

        _dbContext.Set<DebitOriginPermissions>().Add(new DebitOriginPermissions
        {
            ResourceId = originId,
            UserId = userId,
            Resource = origin,
            User = user,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await _dbContext.SaveChangesAsync();

        var command = new DeleteDebitOriginOwnerCommand { EntityId = originId };
        command.SetContext(new FinanceDispatchContext { UserInfo = user });

        var handler = new DeleteDebitOriginOwnerCommandHandler(_dbContext);
        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Empty(_dbContext.Set<DebitOriginPermissions>().ToList());
    }

    [Fact]
    public async Task DeleteOwner_NoMatchingPermissions_ReturnsSuccessWithNoChanges()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "u", FirstName = "F", LastName = "L" };
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var command = new DeleteDebitOriginOwnerCommand { EntityId = Guid.NewGuid() };
        command.SetContext(new FinanceDispatchContext { UserInfo = user });

        var handler = new DeleteDebitOriginOwnerCommandHandler(_dbContext);
        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
    }
}
