using CQRSDispatch;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Services.IOLInvestments;
using Finance.Domain.Enums;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Tests.Services.IOLInvestments;

public partial class IOLInvestmentServiceTests
{
    private static UpdateIOLInvestmentRequest AnUpdateRequest(Guid id) =>
        new(id, "AAPL", 0, 10, 10, 0.5m, 155m, 140m, 7m, 15m, 1550m, IOLInvestmentAssetTypeEnum.Cedear);

    [Fact]
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var investment = new IOLInvestment { Id = Guid.NewGuid() };
        var request = AnUpdateRequest(investment.Id);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestment>>(It.IsAny<UpdateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Success(investment));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(investment, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var id = Guid.NewGuid();
        var request = AnUpdateRequest(id);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestment>>(It.IsAny<UpdateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Success(new IOLInvestment()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IOLInvestment>>(
            It.Is<UpdateIOLInvestmentCommand>(c =>
                c.Id == id &&
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
                c.InvestmentAssetIOLTypeId == request.InvestmentAssetIOLTypeId)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = AnUpdateRequest(Guid.NewGuid());

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IOLInvestment>>(It.IsAny<UpdateIOLInvestmentCommand>()))
            .ReturnsAsync(DataResult<IOLInvestment>.Failure("investment not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("investment not found", result.ErrorMessage);
    }
}
