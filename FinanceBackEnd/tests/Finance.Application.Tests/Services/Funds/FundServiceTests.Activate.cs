using CQRSDispatch;
using Finance.Application.Commands.Funds;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Funds;

public partial class FundServiceTests : IDisposable
{
    [Fact]
    public async Task Activate_DispatchesActivateFundCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<ActivateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Activate(ids);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<ActivateFundCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Activate_ReturnsDispatcherResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<ActivateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Activate([Guid.NewGuid()]);

        Assert.True(result.IsSuccess);
    }
}
