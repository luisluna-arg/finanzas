using CQRSDispatch;
using Finance.Application.Commands.Incomes;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Incomes;

public partial class IncomeServiceTests : IDisposable
{
    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithCorrectResourceId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IncomePermissions>>(It.IsAny<CreateIncomePermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IncomePermissions>.Success(new IncomePermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IncomePermissions>>(
            It.Is<CreateIncomePermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatcherResult()
    {
        var permissions = new IncomePermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IncomePermissions>>(It.IsAny<CreateIncomePermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IncomePermissions>.Success(permissions));

        var result = await _sut.SetOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Equal(permissions, result.Data);
    }
}
