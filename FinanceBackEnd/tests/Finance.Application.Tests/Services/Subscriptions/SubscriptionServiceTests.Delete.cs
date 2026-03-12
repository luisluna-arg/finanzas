using CQRSDispatch;
using Finance.Application.Commands.Subscriptions;
using Finance.Application.Services.Subscriptions;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Subscriptions;

public partial class SubscriptionServiceTests : IDisposable
{
    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteSubscriptionRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var request = new DeleteSubscriptionRequest([id1, id2]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteSubscriptionOwnerCommand>(c => c.EntityId == id1),
            It.IsAny<HttpRequest?>()),
            Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteSubscriptionOwnerCommand>(c => c.EntityId == id2),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteCommandFails_ReturnsFailureAndSkipsOwnerDeletion()
    {
        var request = new DeleteSubscriptionRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete failed"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("Type Subscription delete operation failed", result.ErrorMessage);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteSubscriptionOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new DeleteSubscriptionRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
