using CQRSDispatch;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Services.CreditCards;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.CreditCards;

public partial class CreditCardServiceTests : IDisposable
{
    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid() };
        var request = new DeleteCreditCardRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCreditCardOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var request = new DeleteCreditCardRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCreditCardOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteCreditCardOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Exactly(ids.Length));
    }

    [Fact]
    public async Task Delete_DeletesOwnerWithCorrectEntityId()
    {
        var id = Guid.NewGuid();
        var request = new DeleteCreditCardRequest([id]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCreditCardOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteCreditCardOwnerCommand>(c => c.EntityId == id),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_ReturnsFailure()
    {
        var request = new DeleteCreditCardRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("Type CreditCard delete operation failed", result.ErrorMessage);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_DoesNotDispatchOwnerDelete()
    {
        var request = new DeleteCreditCardRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteCreditCardOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenThrows_ReturnsFailure()
    {
        var request = new DeleteCreditCardRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .Throws(new Exception("unexpected"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected", result.ErrorMessage);
    }
}