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

public sealed class DeleteDebitOwnerCommandHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public DeleteDebitOwnerCommandHandlerTests()
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
        var debitId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "IdentityNotFound" }],
        };
        var debit = new Debit { Id = debitId };
        await _dbContext.User.AddAsync(user);
        await _dbContext.Debit.AddAsync(debit);
        await _dbContext.SaveChangesAsync();

        _dbContext.Set<DebitPermissions>().Add(new DebitPermissions
        {
            ResourceId = debitId,
            UserId = userId,
            Resource = debit,
            User = user,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await _dbContext.SaveChangesAsync();

        var command = new DeleteDebitOwnerCommand { EntityId = debitId };
        command.SetContext(new FinanceDispatchContext { UserInfo = user });

        var handler = new DeleteDebitOwnerCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Empty(_dbContext.Set<DebitPermissions>().IgnoreQueryFilters().ToList());
    }

    [Fact]
    public async Task DeleteOwner_NoMatchingPermissions_ReturnsSuccess()
    {
        var command = new DeleteDebitOwnerCommand { EntityId = Guid.NewGuid() };
        command.SetContext(new FinanceDispatchContext());

        var handler = new DeleteDebitOwnerCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
    }
}
