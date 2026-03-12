using CQRSDispatch;
using Finance.Application.Commands;
using Finance.Application.Commands.Funds;
using Finance.Application.Services.Funds;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Funds;

public partial class FundServiceTests : IDisposable
{
    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteFundRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var request = new DeleteFundRequest([id1, id2]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteFundOwnerCommand>(c => c.EntityId == id1),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteCommandFails_ReturnsFailureAndSkipsOwnerDeletion()
    {
        var request = new DeleteFundRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete failed"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("Type Fund delete operation failed", result.ErrorMessage);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteFundOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new DeleteFundRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundsCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
