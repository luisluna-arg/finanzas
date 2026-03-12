using CQRSDispatch;
using Finance.Application.Commands.Movements;
using Finance.Application.Legacy.Commands.Movements;
using Finance.Application.Services.Movements;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Movements;

public partial class MovementServiceTests
{
    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteMovementRequest([Guid.NewGuid()]);

        SetupDeleteMovementDispatch(CommandResult.Success());
        SetupDeleteOwnerDispatch(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var request = new DeleteMovementRequest(ids);

        SetupDeleteMovementDispatch(CommandResult.Success());
        SetupDeleteOwnerDispatch(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteMovementOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Exactly(ids.Length));
    }

    [Fact]
    public async Task Delete_DeletesOwnerWithCorrectEntityId()
    {
        var id = Guid.NewGuid();
        var request = new DeleteMovementRequest([id]);

        SetupDeleteMovementDispatch(CommandResult.Success());
        SetupDeleteOwnerDispatch(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteMovementOwnerCommand>(c => c.EntityId == id),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_ReturnsFailure()
    {
        var request = new DeleteMovementRequest([Guid.NewGuid()]);

        SetupDeleteMovementDispatch(CommandResult.Failure("delete error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("Type Movement delete operation failed", result.ErrorMessage);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_DoesNotDispatchOwnerDelete()
    {
        var request = new DeleteMovementRequest([Guid.NewGuid()]);

        SetupDeleteMovementDispatch(CommandResult.Failure("delete error"));

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteMovementOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenThrows_ReturnsFailure()
    {
        var request = new DeleteMovementRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteMovementsCommand>()))
            .ThrowsAsync(new Exception("unexpected"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected", result.ErrorMessage);
    }
}
