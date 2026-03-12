using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Services.IOLInvestments;
using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestments;

public partial class IOLInvestmentServiceTests
{
    private static CreateIOLInvestmentRequest ACreateRequest() =>
        new("AAPL", 0, 10, 10, 0.5m, 150m, 140m, 7m, 10m, 1500m, IOLInvestmentAssetTypeEnum.Cedear, null);

    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var investment = new IOLInvestment { Id = Guid.NewGuid() };
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestment>>(It.IsAny<CreateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Success(investment));
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentPermissions>>(It.IsAny<CreateIOLInvestmentPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IOLInvestmentPermissions>.Success(new IOLInvestmentPermissions()));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(investment, result.Data);
    }

    [Fact]
    public async Task Create_DispatchesCommandWithCorrectProperties()
    {
        var request = ACreateRequest();
        var investment = new IOLInvestment { Id = Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestment>>(It.IsAny<CreateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Success(investment));
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentPermissions>>(It.IsAny<CreateIOLInvestmentPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IOLInvestmentPermissions>.Success(new IOLInvestmentPermissions()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IOLInvestment>>(
            It.Is<CreateIOLInvestmentCommand>(c =>
                c.AssetSymbol == request.AssetSymbol &&
                c.Alarms == request.Alarms &&
                c.Quantity == request.Quantity &&
                c.Assets == request.Assets &&
                c.DailyVariation == request.DailyVariation &&
                c.LastPrice == request.LastPrice &&
                c.AverageBuyPrice == request.AverageBuyPrice &&
                c.AverageReturnPercent == request.AverageReturnPercent &&
                c.AverageReturn == request.AverageReturn &&
                c.Valued == request.Valued &&
                c.InvestmentAssetIOLTypeId == request.InvestmentAssetIOLTypeId &&
                c.CurrencyId == request.CurrencyId)),
            Times.Once);
    }

    [Fact]
    public async Task Create_DispatchesPermissionsCommandWithOwnerLevel()
    {
        var investment = new IOLInvestment { Id = Guid.NewGuid() };
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestment>>(It.IsAny<CreateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Success(investment));
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentPermissions>>(It.IsAny<CreateIOLInvestmentPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IOLInvestmentPermissions>.Success(new IOLInvestmentPermissions()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IOLInvestmentPermissions>>(
            It.Is<CreateIOLInvestmentPermissionsCommand>(c =>
                c.ResourceId == investment.Id &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestment>>(It.IsAny<CreateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Failure("dispatch error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("dispatch error", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_DoesNotDispatchPermissions()
    {
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestment>>(It.IsAny<CreateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Failure("dispatch error"));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IOLInvestmentPermissions>>(
            It.IsAny<CreateIOLInvestmentPermissionsCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestment>>(It.IsAny<CreateIOLInvestmentCommand>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
