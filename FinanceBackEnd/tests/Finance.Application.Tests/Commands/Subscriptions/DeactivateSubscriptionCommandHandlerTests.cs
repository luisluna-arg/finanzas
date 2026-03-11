using Finance.Application.Commands.Subscriptions;
using Finance.Application.Tests.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using FinanceBackEnd.Finance.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Tests.Commands.Subscriptions;

public class DeactivateSubscriptionCommandHandlerTests : ActivateDeactivateTestBase
{
    private async Task<Subscription> SeedSubscriptionAsync(bool deactivated)
    {
        var subscription = new Subscription { Id = Guid.NewGuid(), Name = Guid.NewGuid().ToString(), CurrencyId = Guid.NewGuid(), Deactivated = deactivated };
        DbContext.Subscriptions.Add(subscription);
        DbContext.Set<SubscriptionPermissions>().Add(new SubscriptionPermissions
        {
            Id = Guid.NewGuid(),
            ResourceId = subscription.Id,
            Resource = subscription,
            UserId = CurrentUser.Id,
            User = CurrentUser,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await DbContext.SaveChangesAsync();
        return subscription;
    }

    [Fact]
    public async Task Deactivate_ValidIds_DeactivatesEntitiesAndReturnsSuccess()
    {
        var subscription = await SeedSubscriptionAsync(deactivated: false);
        var handler = new DeactivateSubscriptionCommandHandler(DbContext);

        var result = await handler.ExecuteAsync(new DeactivateSubscriptionCommand { Ids = [subscription.Id] }, default);

        Assert.True(result.IsSuccess);
        var updated = await DbContext.Subscriptions.IgnoreQueryFilters().FirstAsync(s => s.Id == subscription.Id);
        Assert.True(updated.Deactivated);
    }

    [Fact]
    public async Task Deactivate_EmptyIds_ThrowsValidationException()
    {
        var handler = new DeactivateSubscriptionCommandHandler(DbContext);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeactivateSubscriptionCommand { Ids = [] }, default));
    }

    [Fact]
    public async Task Deactivate_EmptyGuidInIds_ThrowsValidationException()
    {
        var handler = new DeactivateSubscriptionCommandHandler(DbContext);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeactivateSubscriptionCommand { Ids = [Guid.Empty] }, default));
    }
}
