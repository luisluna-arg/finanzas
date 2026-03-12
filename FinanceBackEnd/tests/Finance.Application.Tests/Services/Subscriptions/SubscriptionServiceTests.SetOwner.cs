using CQRSDispatch;
using Finance.Application.Commands.Subscriptions;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Subscriptions;

public partial class SubscriptionServiceTests : IDisposable
{
    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithCorrectResourceId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(It.IsAny<CreateSubscriptionPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<SubscriptionPermissions>.Success(new SubscriptionPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(
            It.Is<CreateSubscriptionPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatcherResult()
    {
        var permissions = new SubscriptionPermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(It.IsAny<CreateSubscriptionPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<SubscriptionPermissions>.Success(permissions));

        var result = await _sut.SetOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Equal(permissions, result.Data);
    }
}