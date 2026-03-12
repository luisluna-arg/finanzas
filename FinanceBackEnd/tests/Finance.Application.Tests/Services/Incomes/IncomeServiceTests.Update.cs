using CQRSDispatch;
using Finance.Application.Commands.Incomes;
using Finance.Application.Services.Incomes;
using Finance.Domain.Models.Incomes;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Tests.Services.Incomes;

public partial class IncomeServiceTests : IDisposable
{
    [Fact]
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var income = new Income { Id = Guid.NewGuid() };
        var request = new UpdateIncomeRequest(income.Id, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(150m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<UpdateIncomeCommand>()))
            .ReturnsAsync(DataResult<Income>.Success(income));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(income, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var incomeId = Guid.NewGuid();
        var bankId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var timeStamp = DateTime.UtcNow;
        var amount = new Money(500m);
        var request = new UpdateIncomeRequest(incomeId, bankId, currencyId, timeStamp, amount);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<UpdateIncomeCommand>()))
            .ReturnsAsync(DataResult<Income>.Success(new Income()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Income>>(
            It.Is<UpdateIncomeCommand>(c =>
                c.Id == incomeId &&
                c.BankId == bankId &&
                c.CurrencyId == currencyId &&
                c.TimeStamp == timeStamp &&
                c.Amount == amount)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = new UpdateIncomeRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<UpdateIncomeCommand>()))
            .ReturnsAsync(DataResult<Income>.Failure("income not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("income not found", result.ErrorMessage);
    }
}
