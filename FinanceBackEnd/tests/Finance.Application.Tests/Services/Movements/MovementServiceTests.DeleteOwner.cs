using CQRSDispatch;
using Finance.Application.Commands.Movements;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Movements;

public partial class MovementServiceTests
{
    [Fact]
    public async Task DeleteOwner_DispatchesDeleteOwnerCommandWithCorrectId()
    {
        var resourceId = Guid.NewGuid();

        SetupDeleteOwnerDispatch(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteMovementOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteOwner_ReturnsDispatchResult()
    {
        SetupDeleteOwnerDispatch(CommandResult.Success());

        var result = await _sut.DeleteOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
    }
}
