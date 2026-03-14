using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestmentAssets;

public partial class IOLInvestmentAssetServiceTests
{
    [Fact]
    public async Task Activate_DispatchesActivateCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<ActivateIOLInvestmentAssetCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Activate(ids);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<ActivateIOLInvestmentAssetCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
