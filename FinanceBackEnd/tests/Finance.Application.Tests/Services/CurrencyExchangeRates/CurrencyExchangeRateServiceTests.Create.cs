using CQRSDispatch;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Services.CurrencyExchangeRates;
using Finance.Domain.Models.Currencies;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.CurrencyExchangeRates;

public partial class CurrencyExchangeRateServiceTests : IDisposable
{
    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var rate = new CurrencyExchangeRate { Id = Guid.NewGuid() };
        var request = new CreateCurrencyExchangeRateRequest(Guid.NewGuid(), Guid.NewGuid(), 900m, 910m, DateTime.UtcNow);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<CreateCurrencyExchangeRateCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Success(rate));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(rate, result.Data);
    }

    [Fact]
    public async Task Create_DispatchesCommandWithCorrectProperties()
    {
        var baseCurrencyId = Guid.NewGuid();
        var quoteCurrencyId = Guid.NewGuid();
        var buyRate = 900m;
        var sellRate = 910m;
        var timeStamp = DateTime.UtcNow;
        var request = new CreateCurrencyExchangeRateRequest(baseCurrencyId, quoteCurrencyId, buyRate, sellRate, timeStamp);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<CreateCurrencyExchangeRateCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Success(new CurrencyExchangeRate()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(
            It.Is<CreateCurrencyExchangeRateCommand>(c =>
                c.BaseCurrencyId == baseCurrencyId &&
                c.QuoteCurrencyId == quoteCurrencyId &&
                c.BuyRate == buyRate &&
                c.SellRate == sellRate &&
                c.TimeStamp == timeStamp),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateCurrencyExchangeRateRequest(Guid.NewGuid(), Guid.NewGuid(), 900m, 910m, DateTime.UtcNow);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<CreateCurrencyExchangeRateCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Failure("base currency not found"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("base currency not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateCurrencyExchangeRateRequest(Guid.NewGuid(), Guid.NewGuid(), 900m, 910m, DateTime.UtcNow);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<CreateCurrencyExchangeRateCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
