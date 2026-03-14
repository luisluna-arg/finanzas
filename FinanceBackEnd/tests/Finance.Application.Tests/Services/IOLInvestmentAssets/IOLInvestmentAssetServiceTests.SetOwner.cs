using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestmentAssets;

public partial class IOLInvestmentAssetServiceTests
{
    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithOwnerLevel()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentAssetPermissions>>(It.IsAny<CreateIOLInvestmentAssetPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IOLInvestmentAssetPermissions>.Success(new IOLInvestmentAssetPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IOLInvestmentAssetPermissions>>(
            It.Is<CreateIOLInvestmentAssetPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatcherResult()
    {
        var permissions = new IOLInvestmentAssetPermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentAssetPermissions>>(It.IsAny<CreateIOLInvestmentAssetPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IOLInvestmentAssetPermissions>.Success(permissions));

        var result = await _sut.SetOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Equal(permissions, result.Data);
    }
}
