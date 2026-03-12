using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestments;

public partial class IOLInvestmentServiceTests
{
    [Fact]
    public async Task Deactivate_DispatchesDeactivateCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateIOLInvestmentCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Deactivate(ids);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeactivateIOLInvestmentCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
