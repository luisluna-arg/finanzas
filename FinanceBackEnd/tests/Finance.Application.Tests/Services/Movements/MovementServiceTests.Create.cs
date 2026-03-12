using CQRSDispatch;
using Finance.Application.Commands.Movements;
using Finance.Application.Services.Movements;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Movements;
using Finance.Domain.SpecialTypes;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Movements;

public partial class MovementServiceTests
{
    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var movement = new Movement { Id = Guid.NewGuid() };
        var request = BuildCreateRequest();

        SetupCreateMovementDispatch(DataResult<Movement>.Success(movement));
        SetupCreatePermissionsDispatch();

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(movement, result.Data);
    }

    [Fact]
    public async Task Create_DispatchesCommandWithCorrectProperties()
    {
        var appModuleId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        var amount = new Money(150m);
        var total = new Money(160m);
        var request = new CreateMovementRequest(appModuleId, currencyId, timestamp, "Concept A", "Concept B", amount, total);

        SetupCreateMovementDispatch(DataResult<Movement>.Success(new Movement()));
        SetupCreatePermissionsDispatch();

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Movement>>(
            It.Is<CreateMovementCommand>(c =>
                c.AppModuleId == appModuleId &&
                c.CurrencyId == currencyId &&
                c.TimeStamp == timestamp &&
                c.Concept1 == "Concept A" &&
                c.Concept2 == "Concept B" &&
                c.Amount == amount &&
                c.Total == total)),
            Times.Once);
    }

    [Fact]
    public async Task Create_DispatchesPermissionsCommandWithOwnerLevel()
    {
        var movement = new Movement { Id = Guid.NewGuid() };
        var request = BuildCreateRequest();

        SetupCreateMovementDispatch(DataResult<Movement>.Success(movement));
        SetupCreatePermissionsDispatch();

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<MovementPermissions>>(
            It.Is<CreateMovementPermissionsCommand>(c =>
                c.ResourceId == movement.Id &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = BuildCreateRequest();

        SetupCreateMovementDispatch(DataResult<Movement>.Failure("dispatch error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("dispatch error", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_DoesNotDispatchPermissions()
    {
        var request = BuildCreateRequest();

        SetupCreateMovementDispatch(DataResult<Movement>.Failure("dispatch error"));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<MovementPermissions>>(
            It.IsAny<CreateMovementPermissionsCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = BuildCreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Movement>>(It.IsAny<CreateMovementCommand>()))
            .ThrowsAsync(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
