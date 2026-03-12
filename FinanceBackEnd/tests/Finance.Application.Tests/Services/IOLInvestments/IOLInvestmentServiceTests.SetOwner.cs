using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestments;

public partial class IOLInvestmentServiceTests
{
    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithOwnerLevel()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentPermissions>>(It.IsAny<CreateIOLInvestmentPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IOLInvestmentPermissions>.Success(new IOLInvestmentPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IOLInvestmentPermissions>>(
            It.Is<CreateIOLInvestmentPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatcherResult()
    {
        var permissions = new IOLInvestmentPermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentPermissions>>(It.IsAny<CreateIOLInvestmentPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IOLInvestmentPermissions>.Success(permissions));

        var result = await _sut.SetOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Equal(permissions, result.Data);
    }
}
