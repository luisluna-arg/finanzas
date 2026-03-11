using Finance.Application.Commands.Funds;
using Finance.Application.Repositories;
using Finance.Application.Tests.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using FinanceBackEnd.Finance.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Tests.Commands.Funds;

public class FundBatchCommandHandlerTests : ActivateDeactivateTestBase
{
    private readonly Mock<IRepository<Fund, Guid>> _entityService = new();

    private async Task<Fund> SeedFundAsync(bool deactivated)
    {
        var fund = new Fund { Id = Guid.NewGuid(), BankId = Guid.NewGuid(), CurrencyId = Guid.NewGuid(), Deactivated = deactivated };
        DbContext.Fund.Add(fund);
        DbContext.FundPermissions.Add(new FundPermissions
        {
            Id = Guid.NewGuid(),
            ResourceId = fund.Id,
            Resource = fund,
            UserId = CurrentUser.Id,
            User = CurrentUser,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await DbContext.SaveChangesAsync();
        return fund;
    }

    [Fact]
    public async Task Activate_HappyPath_SetsFundsAsActive()
    {
        var fund = await SeedFundAsync(deactivated: true);
        var handler = new ActivateFundCommandHandler(DbContext);

        var result = await handler.ExecuteAsync(new ActivateFundCommand { Ids = [fund.Id] }, default);

        Assert.True(result.IsSuccess);
        var updated = await DbContext.Fund.IgnoreQueryFilters().FirstAsync(f => f.Id == fund.Id);
        Assert.False(updated.Deactivated);
    }

    [Fact]
    public async Task Activate_WhenIdsAreEmpty_ThrowsValidationException()
    {
        var handler = new ActivateFundCommandHandler(DbContext);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new ActivateFundCommand { Ids = [] }, default));
    }

    [Fact]
    public async Task Deactivate_HappyPath_SetsFundsAsDeactivated()
    {
        var fund = await SeedFundAsync(deactivated: false);
        var handler = new DeactivateFundCommandHandler(DbContext);

        var result = await handler.ExecuteAsync(new DeactivateFundCommand { Ids = [fund.Id] }, default);

        Assert.True(result.IsSuccess);
        var updated = await DbContext.Fund.IgnoreQueryFilters().FirstAsync(f => f.Id == fund.Id);
        Assert.True(updated.Deactivated);
    }

    [Fact]
    public async Task Deactivate_WhenAnyIdIsEmpty_ThrowsValidationException()
    {
        var handler = new DeactivateFundCommandHandler(DbContext);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeactivateFundCommand { Ids = [Guid.Empty] }, default));
    }

    [Fact]
    public async Task Delete_HappyPath_DeletesFunds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeleteFundsCommand { Ids = ids };
        var handler = new DeleteFundsCommandHandler(_entityService.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(s => s.DeleteAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>(),
            false), Times.Exactly(ids.Length));
        _entityService.Verify(s => s.PersistAsync(
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Delete_WhenIdsAreEmpty_ThrowsValidationException()
    {
        var handler = new DeleteFundsCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeleteFundsCommand() { Ids = [] }, default));
    }
}