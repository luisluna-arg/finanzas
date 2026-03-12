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
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var fund = new Fund { Id = Guid.NewGuid() };
        var request = new UpdateFundRequest(fund.Id, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(150m), true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<UpdateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Success(fund));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(fund, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var fundId = Guid.NewGuid();
        var bankId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var timeStamp = DateTime.UtcNow;
        var amount = new Money(500m);
        var request = new UpdateFundRequest(fundId, bankId, currencyId, timeStamp, amount, true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<UpdateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Success(new Fund()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Fund>>(
            It.Is<UpdateFundCommand>(c =>
                c.Id == fundId &&
                c.BankId == bankId &&
                c.CurrencyId == currencyId &&
                c.TimeStamp == timeStamp &&
                c.Amount == amount &&
                c.DailyUse == true),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = new UpdateFundRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m), false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<UpdateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Failure("fund not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("fund not found", result.ErrorMessage);
    }
}
