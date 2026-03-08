using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Repositories;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.CurrencyExchangeRates;

public class CreateCurrencyExchangeRateCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepo;
    private readonly Mock<IRepository<CurrencyExchangeRate, Guid>> _rateRepo;
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;

    public CreateCurrencyExchangeRateCommandHandlerTests()
    {
        _currencyRepo = new Mock<IRepository<Currency, Guid>>();
        _rateRepo = new Mock<IRepository<CurrencyExchangeRate, Guid>>();
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task Create_HappyPath_AddsRateAndDispatchesPermissions()
    {
        var baseCurrency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };
        var quoteCurrency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };
        var command = new CreateCurrencyExchangeRateCommand
        {
            BaseCurrencyId = baseCurrency.Id,
            QuoteCurrencyId = quoteCurrency.Id,
            BuyRate = 900m,
            SellRate = 910m,
            TimeStamp = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
        };

        _currencyRepo.Setup(r => r.GetByIdAsync(baseCurrency.Id, It.IsAny<CancellationToken>())).ReturnsAsync(baseCurrency);
        _currencyRepo.Setup(r => r.GetByIdAsync(quoteCurrency.Id, It.IsAny<CancellationToken>())).ReturnsAsync(quoteCurrency);
        _rateRepo
            .Setup(r => r.AddAsync(It.IsAny<CurrencyExchangeRate>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .Callback<CurrencyExchangeRate, CancellationToken, bool>((rate, _, _) => rate.Id = Guid.NewGuid())
            .Returns(Task.CompletedTask);
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRatePermissions>>(
                It.IsAny<CreateCurrencyExchangeRatePermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRatePermissions>.Success(new CurrencyExchangeRatePermissions()));

        var handler = new CreateCurrencyExchangeRateCommandHandler(_dbContext, _currencyRepo.Object, _rateRepo.Object, _dispatcher.Object);
        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(baseCurrency, result.Data.BaseCurrency);
        Assert.Equal(quoteCurrency, result.Data.QuoteCurrency);
        Assert.Equal(900m, (decimal)result.Data.BuyRate);
        Assert.Equal(910m, (decimal)result.Data.SellRate);

        _rateRepo.Verify(r => r.AddAsync(It.IsAny<CurrencyExchangeRate>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CurrencyExchangeRatePermissions>>(
            It.Is<CreateCurrencyExchangeRatePermissionsCommand>(c =>
                c.ResourceId == result.Data.Id &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenBaseCurrencyMissing_ThrowsException()
    {
        var command = new CreateCurrencyExchangeRateCommand
        {
            BaseCurrencyId = Guid.NewGuid(),
            QuoteCurrencyId = Guid.NewGuid(),
            BuyRate = 900m,
            SellRate = 910m,
            TimeStamp = DateTime.UtcNow,
        };

        _currencyRepo.Setup(r => r.GetByIdAsync(command.BaseCurrencyId, It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);

        var handler = new CreateCurrencyExchangeRateCommandHandler(_dbContext, _currencyRepo.Object, _rateRepo.Object, _dispatcher.Object);
        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_WhenQuoteCurrencyMissing_ThrowsException()
    {
        var baseCurrency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };
        var command = new CreateCurrencyExchangeRateCommand
        {
            BaseCurrencyId = baseCurrency.Id,
            QuoteCurrencyId = Guid.NewGuid(),
            BuyRate = 900m,
            SellRate = 910m,
            TimeStamp = DateTime.UtcNow,
        };

        _currencyRepo.Setup(r => r.GetByIdAsync(baseCurrency.Id, It.IsAny<CancellationToken>())).ReturnsAsync(baseCurrency);
        _currencyRepo.Setup(r => r.GetByIdAsync(command.QuoteCurrencyId, It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);

        var handler = new CreateCurrencyExchangeRateCommandHandler(_dbContext, _currencyRepo.Object, _rateRepo.Object, _dispatcher.Object);
        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }
}
