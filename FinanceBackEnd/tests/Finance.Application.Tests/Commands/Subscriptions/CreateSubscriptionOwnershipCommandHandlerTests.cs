using Finance.Application.Auth;
using Finance.Application.Commands.Subscriptions;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Identities;
using Finance.Domain.Models.Subscriptions;
using FinanceBackEnd.Finance.Domain.Enums;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Commands.Subscriptions;

public class CreateSubscriptionOwnershipCommandHandlerTests : QueryHandlerBaseTests
{

    [Fact]
    public async Task CreateOwnership_SubscriptionAndUserFoundByClaimId_ReturnsPermissions()
    {
        const string userIdClaim = "auth0|test-user";
        var subscriptionId = Guid.NewGuid();
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
        await _dbContext.Subscriptions.AddAsync(new Subscription { Id = subscriptionId, Name = "Netflix" });
        await _dbContext.SaveChangesAsync();

        var command = new CreateSubscriptionOwnershipCommand
        {
            ResourceId = subscriptionId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = userIdClaim });

        var handler = new CreateSubscriptionOwnershipCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(subscriptionId, result.Data.ResourceId);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Contains(PermissionLevelEnum.Owner, result.Data.PermissionLevels);
    }

    [Fact]
    public async Task CreateOwnership_ExplicitUserId_FindsUserByIdAndReturnsPermissions()
    {
        var subscriptionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _dbContext.User.AddAsync(new User { Id = userId, Username = "u", FirstName = "F", LastName = "L" });
        await _dbContext.Subscriptions.AddAsync(new Subscription { Id = subscriptionId, Name = "Netflix" });
        await _dbContext.SaveChangesAsync();

        var command = new CreateSubscriptionOwnershipCommand
        {
            ResourceId = subscriptionId,
            UserId = userId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };

        var handler = new CreateSubscriptionOwnershipCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Data.UserId);
    }

    [Fact]
    public async Task CreateOwnership_SubscriptionNotFound_ReturnsFailure()
    {
        var command = new CreateSubscriptionOwnershipCommand
        {
            ResourceId = Guid.NewGuid(),
            PermissionLevels = [PermissionLevelEnum.Owner],
        };

        var handler = new CreateSubscriptionOwnershipCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Resource not found", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateOwnership_UserNotFound_ReturnsFailure()
    {
        var subscriptionId = Guid.NewGuid();
        await _dbContext.Subscriptions.AddAsync(new Subscription { Id = subscriptionId, Name = "Netflix" });
        await _dbContext.SaveChangesAsync();

        var command = new CreateSubscriptionOwnershipCommand
        {
            ResourceId = subscriptionId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = "auth0|unknown" });

        var handler = new CreateSubscriptionOwnershipCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.ErrorMessage);
    }
}
