using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Services.IOLInvestments;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestments;

public partial class IOLInvestmentServiceTests
{
    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteIOLInvestmentRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIOLInvestmentOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var request = new DeleteIOLInvestmentRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIOLInvestmentOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteIOLInvestmentOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Exactly(ids.Length));
    }

    [Fact]
    public async Task Delete_DeletesOwnerWithCorrectEntityId()
    {
        var id = Guid.NewGuid();
        var request = new DeleteIOLInvestmentRequest([id]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIOLInvestmentOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteIOLInvestmentOwnerCommand>(c => c.EntityId == id),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_ReturnsFailure()
    {
        var request = new DeleteIOLInvestmentRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentCommand>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("Type IOLInvestment delete operation failed", result.ErrorMessage);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_DoesNotDispatchOwnerDelete()
    {
        var request = new DeleteIOLInvestmentRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentCommand>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteIOLInvestmentOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new DeleteIOLInvestmentRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentCommand>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
