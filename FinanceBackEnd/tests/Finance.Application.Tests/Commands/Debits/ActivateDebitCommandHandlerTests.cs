using Finance.Application.Commands.Debits;
using Finance.Application.Tests.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using FinanceBackEnd.Finance.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Tests.Commands.Debits;

public class ActivateDebitCommandHandlerTests : ActivateDeactivateTestBase
{
    private async Task<Debit> SeedDebitAsync(bool deactivated)
    {
        var origin = new DebitOrigin { Id = Guid.NewGuid(), Name = "Origin", AppModuleId = Guid.NewGuid() };
        var debit = new Debit { Id = Guid.NewGuid(), Origin = origin, OriginId = origin.Id, Deactivated = deactivated };
        DbContext.Debit.Add(debit);
        DbContext.DebitPermissions.Add(new DebitPermissions
        {
            Id = Guid.NewGuid(),
            ResourceId = debit.Id,
            Resource = debit,
            UserId = CurrentUser.Id,
            User = CurrentUser,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await DbContext.SaveChangesAsync();
        return debit;
    }

    [Fact]
    public async Task Activate_ValidIds_ActivatesEntitiesAndReturnsSuccess()
    {
        var debit = await SeedDebitAsync(deactivated: true);
        var handler = new ActivateDebitCommandHandler(DbContext);

        var result = await handler.ExecuteAsync(new ActivateDebitCommand { Ids = [debit.Id] }, default);

        Assert.True(result.IsSuccess);
        var updated = await DbContext.Debit.IgnoreQueryFilters().FirstAsync(d => d.Id == debit.Id);
        Assert.False(updated.Deactivated);
    }

    [Fact]
    public async Task Activate_EmptyIds_ThrowsValidationException()
    {
        var handler = new ActivateDebitCommandHandler(DbContext);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new ActivateDebitCommand { Ids = [] }, default));
    }

    [Fact]
    public async Task Activate_EmptyGuidInIds_ThrowsValidationException()
    {
        var handler = new ActivateDebitCommandHandler(DbContext);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new ActivateDebitCommand { Ids = [Guid.Empty] }, default));
    }
}
