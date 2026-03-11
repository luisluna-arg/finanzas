using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.CreditCards;

public sealed class DeleteCreditCardOwnerCommandHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public DeleteCreditCardOwnerCommandHandlerTests()
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
        var creditCardId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "IdentityNotFound" }],
        };
        var creditCard = new CreditCard { Id = creditCardId };
        await _dbContext.User.AddAsync(user);
        await _dbContext.CreditCard.AddAsync(creditCard);
        await _dbContext.SaveChangesAsync();

        _dbContext.Set<CreditCardPermissions>().Add(new CreditCardPermissions
        {
            ResourceId = creditCardId,
            UserId = userId,
            Resource = creditCard,
            User = user,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await _dbContext.SaveChangesAsync();

        var command = new DeleteCreditCardOwnerCommand { EntityId = creditCardId };
        command.SetContext(new FinanceDispatchContext { UserInfo = user });

        var handler = new DeleteCreditCardOwnerCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Empty(_dbContext.Set<CreditCardPermissions>().IgnoreQueryFilters().ToList());
    }

    [Fact]
    public async Task DeleteOwner_NoMatchingPermissions_ReturnsSuccess()
    {
        var command = new DeleteCreditCardOwnerCommand { EntityId = Guid.NewGuid() };
        command.SetContext(new FinanceDispatchContext());

        var handler = new DeleteCreditCardOwnerCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteOwner_OnlyDeletesPermissionsForMatchingEntityAndUser()
    {
        var creditCardIdA = Guid.NewGuid();
        var creditCardIdB = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var user = new User { Id = userId, Username = "u", FirstName = "F", LastName = "L", Identities = [new Identity { SourceId = "IdentityNotFound" }] };
        var otherUser = new User { Id = otherUserId, Username = "v", FirstName = "G", LastName = "H" };
        var cardA = new CreditCard { Id = creditCardIdA };
        var cardB = new CreditCard { Id = creditCardIdB };

        await _dbContext.User.AddRangeAsync(user, otherUser);
        await _dbContext.CreditCard.AddRangeAsync(cardA, cardB);
        await _dbContext.SaveChangesAsync();

        _dbContext.Set<CreditCardPermissions>().AddRange(
            new CreditCardPermissions
            {
                ResourceId = creditCardIdA,
                UserId = userId,
                Resource = cardA,
                User = user,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            new CreditCardPermissions
            {
                ResourceId = creditCardIdB,
                UserId = userId,
                Resource = cardB,
                User = user,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            new CreditCardPermissions
            {
                ResourceId = creditCardIdA,
                UserId = otherUserId,
                Resource = cardA,
                User = otherUser,
                PermissionLevels = [PermissionLevelEnum.Owner],
            });
        await _dbContext.SaveChangesAsync();

        var command = new DeleteCreditCardOwnerCommand { EntityId = creditCardIdA };
        command.SetContext(new FinanceDispatchContext { UserInfo = user });

        var handler = new DeleteCreditCardOwnerCommandHandler(_dbContext);
        await handler.ExecuteAsync(command);

        var remaining = _dbContext.Set<CreditCardPermissions>().IgnoreQueryFilters().ToList();
        Assert.Equal(2, remaining.Count);
        Assert.DoesNotContain(remaining, p => p.ResourceId == creditCardIdA && p.UserId == userId);
    }
}
