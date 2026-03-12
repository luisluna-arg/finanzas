using CQRSDispatch;
using Finance.Application.Commands.Movements;

namespace Finance.Application.Tests.Services.Movements;

public partial class MovementServiceTests
{
    [Fact]
    public async Task Activate_DispatchesActivateCommandForEachId()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<ActivateMovementCommand>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Activate(ids);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchCommandAsync(It.IsAny<ActivateMovementCommand>()), Times.Exactly(ids.Length));
    }

    [Fact]
    public async Task Activate_DispatchesWithCorrectId()
    {
        var id = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<ActivateMovementCommand>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Activate([id]);

        _dispatcher.Verify(d => d.DispatchCommandAsync(
            It.Is<ActivateMovementCommand>(c => c.Id == id)),
            Times.Once);
    }
}
