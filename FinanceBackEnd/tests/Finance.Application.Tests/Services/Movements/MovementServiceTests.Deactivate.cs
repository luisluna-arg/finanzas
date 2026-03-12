using CQRSDispatch;
using Finance.Application.Commands.Movements;

namespace Finance.Application.Tests.Services.Movements;

public partial class MovementServiceTests
{
    [Fact]
    public async Task Deactivate_DispatchesDeactivateCommandForEachId()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeactivateMovementCommand>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Deactivate(ids);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchCommandAsync(It.IsAny<DeactivateMovementCommand>()), Times.Exactly(ids.Length));
    }

    [Fact]
    public async Task Deactivate_DispatchesWithCorrectId()
    {
        var id = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeactivateMovementCommand>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Deactivate([id]);

        _dispatcher.Verify(d => d.DispatchCommandAsync(
            It.Is<DeactivateMovementCommand>(c => c.Id == id)),
            Times.Once);
    }
}
