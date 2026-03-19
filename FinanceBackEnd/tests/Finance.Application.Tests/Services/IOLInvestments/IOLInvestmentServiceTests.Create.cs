using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Services.IOLInvestments;
using Finance.Domain.Enums;
using Finance.Domain.Models.IOLInvestments;

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
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Success(investment));

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
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Success(investment));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync(
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
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Failure("dispatch error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("dispatch error", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = ACreateRequest();

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateIOLInvestmentCommand>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
