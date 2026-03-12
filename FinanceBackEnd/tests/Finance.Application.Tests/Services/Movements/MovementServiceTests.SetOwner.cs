using CQRSDispatch;
using Finance.Application.Commands.Movements;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Movements;

public partial class MovementServiceTests
{
    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithOwnerLevel()
    {
        var resourceId = Guid.NewGuid();

        SetupCreatePermissionsDispatch();

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<MovementPermissions>>(
            It.Is<CreateMovementPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatchResult()
    {
        var permissions = new MovementPermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<MovementPermissions>>(It.IsAny<CreateMovementPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<MovementPermissions>.Success(permissions));

        var result = await _sut.SetOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Equal(permissions, result.Data);
    }
}
