using CQRSDispatch;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.CurrencyExchangeRates;

public partial class CurrencyExchangeRateServiceTests : IDisposable
{
    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithOwner()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRatePermissions>>(
                It.IsAny<CreateCurrencyExchangeRatePermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRatePermissions>.Success(new CurrencyExchangeRatePermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CurrencyExchangeRatePermissions>>(
            It.Is<CreateCurrencyExchangeRatePermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
