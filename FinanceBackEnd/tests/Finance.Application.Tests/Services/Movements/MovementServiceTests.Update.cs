using CQRSDispatch;
using Finance.Application.Commands.Movements;
using Finance.Application.Services.Movements;
using Finance.Domain.Models.Movements;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Tests.Services.Movements;

public partial class MovementServiceTests
{
    [Fact]
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var movement = new Movement { Id = Guid.NewGuid() };
        var request = BuildUpdateRequest(movement.Id);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Movement>>(It.IsAny<PartialUpdateMovementCommand>()))
            .ReturnsAsync(DataResult<Movement>.Success(movement));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(movement, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var id = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        var amount = new Money(200m);
        var total = new Money(210m);
        var request = new UpdateMovementRequest(id, timestamp, "Updated Concept", null, amount, total);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Movement>>(It.IsAny<PartialUpdateMovementCommand>()))
            .ReturnsAsync(DataResult<Movement>.Success(new Movement()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Movement>>(
            It.Is<PartialUpdateMovementCommand>(c =>
                c.Id == id &&
                c.TimeStamp == timestamp &&
                c.Concept1 == "Updated Concept" &&
                c.Concept2 == null &&
                c.Amount == amount &&
                c.Total == total)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = BuildUpdateRequest(Guid.NewGuid());

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Movement>>(It.IsAny<PartialUpdateMovementCommand>()))
            .ReturnsAsync(DataResult<Movement>.Failure("not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("not found", result.ErrorMessage);
    }
}
