using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestmentAssets;

public partial class IOLInvestmentAssetServiceTests
{
    [Fact]
    public async Task DeleteOwner_DispatchesDeleteOwnerCommandWithCorrectEntityId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIOLInvestmentAssetOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteIOLInvestmentAssetOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteOwner_ReturnsDispatcherResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIOLInvestmentAssetOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.DeleteOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
    }
}
