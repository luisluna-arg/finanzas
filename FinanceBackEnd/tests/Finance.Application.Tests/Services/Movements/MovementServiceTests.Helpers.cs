using CQRSDispatch;
using Finance.Application.Commands.Movements;
using Finance.Application.Services.Movements;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Movements;
using Finance.Domain.SpecialTypes;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Movements;

public partial class MovementServiceTests
{
    private static CreateMovementRequest BuildCreateRequest() =>
        new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "Salary", null, new Money(1000m), null);

    private static UpdateMovementRequest BuildUpdateRequest(Guid id) =>
        new(id, DateTime.UtcNow, "Updated", null, new Money(500m), null);

    private void SetupCreatePermissionsDispatch() =>
        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateMovementPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<MovementPermissions>.Success(new MovementPermissions()));

    private void SetupCreateMovementDispatch(DataResult<Movement> result) =>
        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateMovementCommand>()))
            .ReturnsAsync(result);

    private void SetupDeleteMovementDispatch(CommandResult result) =>
        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteMovementsCommand>()))
            .ReturnsAsync(result);

    private void SetupDeleteOwnerDispatch(CommandResult result) =>
        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<DeleteMovementOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(result);
}
