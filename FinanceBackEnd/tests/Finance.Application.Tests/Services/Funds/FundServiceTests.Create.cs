using CQRSDispatch;
using Finance.Application.Commands.Funds;
using Finance.Application.Services.Funds;
using Finance.Domain.Models.Funds;
using Finance.Domain.SpecialTypes;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Funds;

public partial class FundServiceTests : IDisposable
{
    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var fund = new Fund { Id = Guid.NewGuid() };
        var request = new CreateFundRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m), true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<CreateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Success(fund));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(fund, result.Data);
    }

    [Fact]
    public async Task Create_WhenDispatchSucceeds_DispatchesCommandWithCorrectProperties()
    {
        var bankId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var timeStamp = DateTime.UtcNow;
        var amount = new Money(250m);
        var request = new CreateFundRequest(bankId, currencyId, timeStamp, amount, false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<CreateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Success(new Fund()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Fund>>(
            It.Is<CreateFundCommand>(c =>
                c.BankId == bankId &&
                c.CurrencyId == currencyId &&
                c.TimeStamp == timeStamp &&
                c.Amount == amount &&
                c.DailyUse == false),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateFundRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m), true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<CreateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Failure("bank not found"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("bank not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateFundRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m), true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<CreateFundCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
