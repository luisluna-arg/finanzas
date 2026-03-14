using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Services.IOLInvestmentAssets;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestmentAssets;

public partial class IOLInvestmentAssetServiceTests
{
    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteIOLInvestmentAssetRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentAssetCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIOLInvestmentAssetOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var request = new DeleteIOLInvestmentAssetRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentAssetCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIOLInvestmentAssetOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteIOLInvestmentAssetOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Exactly(ids.Length));
    }

    [Fact]
    public async Task Delete_DeletesOwnerWithCorrectEntityId()
    {
        var id = Guid.NewGuid();
        var request = new DeleteIOLInvestmentAssetRequest([id]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentAssetCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIOLInvestmentAssetOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteIOLInvestmentAssetOwnerCommand>(c => c.EntityId == id),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_ReturnsFailure()
    {
        var request = new DeleteIOLInvestmentAssetRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentAssetCommand>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_DoesNotDeleteOwners()
    {
        var request = new DeleteIOLInvestmentAssetRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteIOLInvestmentAssetCommand>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteIOLInvestmentAssetOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }
}
