using CQRSDispatch;
using Finance.Application.Commands.Funds;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Funds;

public partial class FundServiceTests : IDisposable
{
    [Fact]
    public async Task Deactivate_DispatchesDeactivateFundCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Deactivate(ids);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeactivateFundCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Deactivate_ReturnsDispatcherResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Deactivate([Guid.NewGuid()]);

        Assert.True(result.IsSuccess);
    }
}
