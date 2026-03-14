using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Services.IOLInvestmentAssets;
using Finance.Domain.Enums;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Tests.Services.IOLInvestmentAssets;

public partial class IOLInvestmentAssetServiceTests
{
    private static UpdateIOLInvestmentAssetRequest AnUpdateRequest(Guid id) =>
        new(id, IOLInvestmentAssetTypeEnum.Cedear, Guid.NewGuid(), "MSFT", "Microsoft Corp.");

    [Fact]
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var asset = new IOLInvestmentAsset { Id = Guid.NewGuid() };
        var request = AnUpdateRequest(asset.Id);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentAsset>>(It.IsAny<UpdateIOLInvestmentAssetCommand>()))
            .ReturnsAsync(DataResult<IOLInvestmentAsset>.Success(asset));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(asset, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var id = Guid.NewGuid();
        var request = AnUpdateRequest(id);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentAsset>>(It.IsAny<UpdateIOLInvestmentAssetCommand>()))
            .ReturnsAsync(DataResult<IOLInvestmentAsset>.Success(new IOLInvestmentAsset()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IOLInvestmentAsset>>(
            It.Is<UpdateIOLInvestmentAssetCommand>(c =>
                c.Id == id &&
                c.TypeId == request.TypeId &&
                c.CurrencyId == request.CurrencyId &&
                c.Symbol == request.Symbol &&
                c.Description == request.Description)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = AnUpdateRequest(Guid.NewGuid());

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestmentAsset>>(It.IsAny<UpdateIOLInvestmentAssetCommand>()))
            .ReturnsAsync(DataResult<IOLInvestmentAsset>.Failure("update error"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("update error", result.ErrorMessage);
    }
}
