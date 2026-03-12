using CQRSDispatch;
using Finance.Application.Commands.Debits;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Debits;

public partial class DebitServiceTests : IDisposable
{
    [Fact]
    public async Task Deactivate_DispatchesDeactivateCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Deactivate(ids);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeactivateDebitCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
