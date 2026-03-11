using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Tests.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using FinanceBackEnd.Finance.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Tests.Commands.DebitOrigins;

public class ActivateDebitOriginCommandHandlerTests : ActivateDeactivateTestBase
{
    private async Task<DebitOrigin> SeedDebitOriginAsync(bool deactivated)
    {
        var origin = new DebitOrigin { Id = Guid.NewGuid(), Name = "Origin", AppModuleId = Guid.NewGuid(), Deactivated = deactivated };
        DbContext.DebitOrigin.Add(origin);
        DbContext.DebitOriginPermissions.Add(new DebitOriginPermissions
        {
            Id = Guid.NewGuid(),
            ResourceId = origin.Id,
            Resource = origin,
            UserId = CurrentUser.Id,
            User = CurrentUser,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await DbContext.SaveChangesAsync();
        return origin;
    }

    [Fact]
    public async Task Activate_ValidIds_ActivatesEntitiesAndReturnsSuccess()
    {
        var origin = await SeedDebitOriginAsync(deactivated: true);
        var handler = new ActivateDebitOriginCommandHandler(DbContext);

        var result = await handler.ExecuteAsync(new ActivateDebitOriginCommand { Ids = [origin.Id] }, default);

        Assert.True(result.IsSuccess);
        var updated = await DbContext.DebitOrigin.IgnoreQueryFilters().FirstAsync(o => o.Id == origin.Id);
        Assert.False(updated.Deactivated);
    }

    [Fact]
    public async Task Activate_EmptyIds_ThrowsValidationException()
    {
        var handler = new ActivateDebitOriginCommandHandler(DbContext);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new ActivateDebitOriginCommand { Ids = [] }, default));
    }

    [Fact]
    public async Task Activate_EmptyGuidInIds_ThrowsValidationException()
    {
        var handler = new ActivateDebitOriginCommandHandler(DbContext);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new ActivateDebitOriginCommand { Ids = [Guid.Empty] }, default));
    }
}
