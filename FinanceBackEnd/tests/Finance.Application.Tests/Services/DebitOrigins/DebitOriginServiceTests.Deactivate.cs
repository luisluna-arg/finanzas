using CQRSDispatch;
using Finance.Application.Commands.DebitOrigins;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.DebitOrigins;

public partial class DebitOriginServiceTests : IDisposable
{
    [Fact]
    public async Task Deactivate_DispatchesDeactivateCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Deactivate(ids);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeactivateDebitOriginCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
