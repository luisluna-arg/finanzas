using CQRSDispatch;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Services.CurrencyExchangeRates;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Tests.Services.CurrencyExchangeRates;

public partial class CurrencyExchangeRateServiceTests : IDisposable
{
    [Fact]
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var rate = new CurrencyExchangeRate { Id = Guid.NewGuid() };
        var request = new UpdateCurrencyExchangeRateRequest(rate.Id, 900m, 910m);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<UpdateCurrencyExchangeRateCommand>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Success(rate));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(rate, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var id = Guid.NewGuid();
        var buyRate = 900m;
        var sellRate = 910m;
        var request = new UpdateCurrencyExchangeRateRequest(id, buyRate, sellRate);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<UpdateCurrencyExchangeRateCommand>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Success(new CurrencyExchangeRate()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(
            It.Is<UpdateCurrencyExchangeRateCommand>(c =>
                c.Id == id &&
                c.BuyRate == buyRate &&
                c.SellRate == sellRate)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = new UpdateCurrencyExchangeRateRequest(Guid.NewGuid(), 900m, 910m);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<UpdateCurrencyExchangeRateCommand>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Failure("rate not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("rate not found", result.ErrorMessage);
    }
}
