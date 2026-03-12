using CQRSDispatch;
using Finance.Application.Commands.CreditCards;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.CreditCards;

public partial class CreditCardServiceTests : IDisposable
{
    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithOwnerLevel()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(It.IsAny<CreateCreditCardPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CreditCardPermissions>.Success(new CreditCardPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(
            It.Is<CreateCreditCardPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatchResult()
    {
        var permissions = new CreditCardPermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(It.IsAny<CreateCreditCardPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CreditCardPermissions>.Success(permissions));

        var result = await _sut.SetOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Equal(permissions, result.Data);
    }
}