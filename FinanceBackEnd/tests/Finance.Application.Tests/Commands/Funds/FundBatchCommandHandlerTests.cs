using Finance.Application.Commands.Funds;
using Finance.Application.Services;
using Finance.Domain.Models.Funds;
using FluentValidation;

namespace Finance.Application.Tests.Commands.Funds;

public class FundBatchCommandHandlerTests
{
    private readonly Mock<IEntityService<Fund, Guid>> _entityService;

    public FundBatchCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<Fund, Guid>>();
    }

    [Fact]
    public async Task Activate_HappyPath_SetsFundsAsActive()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new ActivateFundCommand { Ids = ids };
        var handler = new ActivateFundCommandHandler(_entityService.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(s => s.SetDeactivatedAsync(ids, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Activate_WhenIdsAreEmpty_ThrowsValidationException()
    {
        var handler = new ActivateFundCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(new ActivateFundCommand { Ids = [] }, default));
    }

    [Fact]
    public async Task Deactivate_HappyPath_SetsFundsAsDeactivated()
    {
        var ids = new[] { Guid.NewGuid() };
        var command = new DeactivateFundCommand { Ids = ids };
        var handler = new DeactivateFundCommandHandler(_entityService.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(s => s.SetDeactivatedAsync(ids, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deactivate_WhenAnyIdIsEmpty_ThrowsValidationException()
    {
        var handler = new DeactivateFundCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(new DeactivateFundCommand { Ids = [Guid.Empty] }, default));
    }

    [Fact]
    public async Task Delete_HappyPath_DeletesFunds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeleteFundsCommand { Ids = ids };
        var handler = new DeleteFundsCommandHandler(_entityService.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(s => s.DeleteAsync(ids, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenIdsAreEmpty_ThrowsValidationException()
    {
        var handler = new DeleteFundsCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(new DeleteFundsCommand { Ids = [] }, default));
    }
}