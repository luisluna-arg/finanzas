using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestmentAssets;

public partial class IOLInvestmentAssetServiceTests
{
    [Fact]
    public async Task Deactivate_DispatchesDeactivateCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateIOLInvestmentAssetCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Deactivate(ids);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeactivateIOLInvestmentAssetCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
