using CQRSDispatch;
using Finance.Application.Commands.Incomes;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Incomes;

public partial class IncomeServiceTests : IDisposable
{
    [Fact]
    public async Task DeleteOwner_DispatchesDeleteOwnerCommandWithCorrectEntityId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomeOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteIncomeOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteOwner_ReturnsDispatcherResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomeOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.DeleteOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
    }
}
