using Finance.Application.Auth;
using Finance.Application.Commands.Subscriptions;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Identities;
using Finance.Domain.Models.Subscriptions;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Commands.Subscriptions;

public sealed class DeleteSubscriptionOwnerCommandHandlerTests : QueryHandlerBaseTests
{

    [Fact]
    public async Task DeleteOwner_MatchingPermissionsExist_DeletesAndReturnsSuccess()
    {
        var subscriptionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Identity SourceId must match FinanceDbContext.CurrentUserId ("IdentityNotFound" when no HttpContextAccessor)
        // so the query filter on SubscriptionPermissions passes and the handler can see the records.
        var user = new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "IdentityNotFound" }],
        };
        var subscription = new Subscription { Id = subscriptionId, Name = "Netflix" };
        await _dbContext.User.AddAsync(user);
        await _dbContext.Subscriptions.AddAsync(subscription);
        await _dbContext.SaveChangesAsync();

        _dbContext.Set<SubscriptionPermissions>().Add(new SubscriptionPermissions
        {
            ResourceId = subscriptionId,
            UserId = userId,
            Resource = subscription,
            User = user,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await _dbContext.SaveChangesAsync();

        var command = new DeleteSubscriptionOwnerCommand { EntityId = subscriptionId };
        command.SetContext(new FinanceDispatchContext { UserInfo = user });

        var handler = new DeleteSubscriptionOwnerCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Empty(_dbContext.Set<SubscriptionPermissions>().IgnoreQueryFilters().ToList());
    }

    [Fact]
    public async Task DeleteOwner_NoMatchingPermissions_ReturnsSuccess()
    {
        var command = new DeleteSubscriptionOwnerCommand { EntityId = Guid.NewGuid() };
        command.SetContext(new FinanceDispatchContext());

        var handler = new DeleteSubscriptionOwnerCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
    }
}
