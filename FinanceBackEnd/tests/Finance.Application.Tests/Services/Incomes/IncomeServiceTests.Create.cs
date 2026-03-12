using CQRSDispatch;
using Finance.Application.Commands.Incomes;
using Finance.Application.Services.Incomes;
using Finance.Domain.Models.Incomes;
using Finance.Domain.SpecialTypes;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Incomes;

public partial class IncomeServiceTests : IDisposable
{
    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var income = new Income { Id = Guid.NewGuid() };
        var request = new CreateIncomeRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<CreateIncomeCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Income>.Success(income));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(income, result.Data);
    }

    [Fact]
    public async Task Create_WhenDispatchSucceeds_DispatchesCommandWithCorrectProperties()
    {
        var bankId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var timeStamp = DateTime.UtcNow;
        var amount = new Money(250m);
        var request = new CreateIncomeRequest(bankId, currencyId, timeStamp, amount);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<CreateIncomeCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Income>.Success(new Income()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Income>>(
            It.Is<CreateIncomeCommand>(c =>
                c.BankId == bankId &&
                c.CurrencyId == currencyId &&
                c.TimeStamp == timeStamp &&
                c.Amount == amount),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateIncomeRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<CreateIncomeCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Income>.Failure("bank not found"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("bank not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateIncomeRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<CreateIncomeCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
