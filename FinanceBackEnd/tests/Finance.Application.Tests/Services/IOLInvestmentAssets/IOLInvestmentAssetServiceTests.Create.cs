using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Services.IOLInvestmentAssets;
using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.IOLInvestmentAssets;

public partial class IOLInvestmentAssetServiceTests
{
    private static CreateIOLInvestmentAssetRequest ACreateRequest() =>
        new(IOLInvestmentAssetTypeEnum.Cedear, Guid.NewGuid(), "AAPL", "Apple Inc.");

    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var asset = new IOLInvestmentAsset { Id = Guid.NewGuid() };
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentAssetCommand>()))
            .ReturnsAsync(DataResult<IOLInvestmentAsset>.Success(asset));
        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentAssetPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IOLInvestmentAssetPermissions>.Success(new IOLInvestmentAssetPermissions()));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(asset, result.Data);
    }

    [Fact]
    public async Task Create_DispatchesCommandWithCorrectProperties()
    {
        var request = ACreateRequest();
        var asset = new IOLInvestmentAsset { Id = Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentAssetCommand>()))
            .ReturnsAsync(DataResult<IOLInvestmentAsset>.Success(asset));
        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentAssetPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IOLInvestmentAssetPermissions>.Success(new IOLInvestmentAssetPermissions()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync(
            It.Is<CreateIOLInvestmentAssetCommand>(c =>
                c.TypeId == request.TypeId &&
                c.CurrencyId == request.CurrencyId &&
                c.Symbol == request.Symbol &&
                c.Description == request.Description)),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenCommandDispatchFails_ReturnsFailure()
    {
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentAssetCommand>()))
            .ReturnsAsync(DataResult<IOLInvestmentAsset>.Failure("create error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("create error", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenCommandDispatchFails_DoesNotDispatchPermissions()
    {
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentAssetCommand>()))
            .ReturnsAsync(DataResult<IOLInvestmentAsset>.Failure("create error"));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync(
            It.IsAny<CreateIOLInvestmentAssetPermissionsCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }
}
