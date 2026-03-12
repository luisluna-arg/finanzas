using CQRSDispatch;
using Finance.Application.Commands.DebitOrigins;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.DebitOrigins;

public partial class DebitOriginServiceTests : IDisposable
{
    [Fact]
    public async Task DeleteOwner_DispatchesDeleteOwnerCommandWithCorrectId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOriginOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteDebitOriginOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
