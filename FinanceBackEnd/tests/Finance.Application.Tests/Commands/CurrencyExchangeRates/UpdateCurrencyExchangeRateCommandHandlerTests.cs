using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Commands.CurrencyExchangeRates;

public class UpdateCurrencyExchangeRateCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<CurrencyExchangeRate, Guid>> _rateRepo;

    public UpdateCurrencyExchangeRateCommandHandlerTests()
    {
        _rateRepo = new Mock<IRepository<CurrencyExchangeRate, Guid>>();
    }

    [Fact]
    public async Task Update_HappyPath_UpdatesRatesAndReturnsSuccess()
    {
        var rate = new CurrencyExchangeRate
        {
            Id = Guid.NewGuid(),
            BuyRate = 100m,
            SellRate = 105m,
        };
        var command = new UpdateCurrencyExchangeRateCommand
        {
            Id = rate.Id,
            BuyRate = 910m,
            SellRate = 920m,
        };

        _rateRepo.Setup(r => r.GetByIdAsync(rate.Id, It.IsAny<CancellationToken>())).ReturnsAsync(rate);

        var handler = new UpdateCurrencyExchangeRateCommandHandler(_dbContext, _rateRepo.Object);
        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(910m, (decimal)result.Data.BuyRate);
        Assert.Equal(920m, (decimal)result.Data.SellRate);

        _rateRepo.Verify(r => r.UpdateAsync(rate, It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Update_WhenRateNotFound_ThrowsException()
    {
        var command = new UpdateCurrencyExchangeRateCommand
        {
            Id = Guid.NewGuid(),
            BuyRate = 910m,
            SellRate = 920m,
        };

        _rateRepo.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync((CurrencyExchangeRate?)null);

        var handler = new UpdateCurrencyExchangeRateCommandHandler(_dbContext, _rateRepo.Object);
        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }
}
