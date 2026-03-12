using CQRSDispatch;
using Finance.Application.Commands.Funds;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Funds;

public partial class FundServiceTests : IDisposable
{
    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithCorrectResourceId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<FundPermissions>>(It.IsAny<CreateFundPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<FundPermissions>.Success(new FundPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<FundPermissions>>(
            It.Is<CreateFundPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatcherResult()
    {
        var permissions = new FundPermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<FundPermissions>>(It.IsAny<CreateFundPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<FundPermissions>.Success(permissions));

        var result = await _sut.SetOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Equal(permissions, result.Data);
    }
}
