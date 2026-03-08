using Finance.Application.Commands.Funds;
using Finance.Application.Repositories;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Funds;
using Finance.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Funds;

public class UpdateFundCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<Fund, Guid>> _fundRepo;
    private readonly Mock<IRepository<Bank, Guid>> _bankRepo;
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepo;
    private readonly FinanceDbContext _dbContext;

    public UpdateFundCommandHandlerTests()
    {
        _fundRepo = new Mock<IRepository<Fund, Guid>>();
        _bankRepo = new Mock<IRepository<Bank, Guid>>();
        _currencyRepo = new Mock<IRepository<Currency, Guid>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task Update_HappyPath_UpdatesFundAndReturnsSuccess()
    {
        var bank = new Bank { Id = Guid.NewGuid(), Name = "BBVA" };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };
        var fund = new Fund
        {
            Id = Guid.NewGuid(),
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            Amount = 10m,
            TimeStamp = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            DailyUse = false,
        };
        var command = new UpdateFundCommand
        {
            Id = fund.Id,
            BankId = bank.Id,
            CurrencyId = currency.Id,
            Amount = 55m,
            TimeStamp = new DateTime(2025, 2, 2, 0, 0, 0, DateTimeKind.Utc),
            DailyUse = true,
        };

        _fundRepo.Setup(r => r.GetByIdAsync(fund.Id, It.IsAny<CancellationToken>())).ReturnsAsync(fund);
        _bankRepo.Setup(r => r.GetByIdAsync(bank.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bank);
        _currencyRepo.Setup(r => r.GetByIdAsync(currency.Id, It.IsAny<CancellationToken>())).ReturnsAsync(currency);

        var handler = new UpdateFundCommandHandler(_dbContext, _fundRepo.Object, _bankRepo.Object, _currencyRepo.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(bank, result.Data.Bank);
        Assert.Equal(currency, result.Data.Currency);
        Assert.Equal(55m, (decimal)result.Data.Amount);
        Assert.Equal(command.TimeStamp, result.Data.TimeStamp);
        Assert.True(result.Data.DailyUse);

        _fundRepo.Verify(r => r.UpdateAsync(fund, It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Update_WhenFundIsMissing_ThrowsException()
    {
        var command = new UpdateFundCommand
        {
            Id = Guid.NewGuid(),
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            Amount = 10m,
            TimeStamp = DateTime.UtcNow,
        };

        _fundRepo.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Fund?)null);

        var handler = new UpdateFundCommandHandler(_dbContext, _fundRepo.Object, _bankRepo.Object, _currencyRepo.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_WhenCurrencyIsMissing_ThrowsException()
    {
        var fund = new Fund { Id = Guid.NewGuid() };
        var command = new UpdateFundCommand
        {
            Id = fund.Id,
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            Amount = 10m,
            TimeStamp = DateTime.UtcNow,
        };

        _fundRepo.Setup(r => r.GetByIdAsync(fund.Id, It.IsAny<CancellationToken>())).ReturnsAsync(fund);
        _currencyRepo.Setup(r => r.GetByIdAsync(command.CurrencyId, It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);

        var handler = new UpdateFundCommandHandler(_dbContext, _fundRepo.Object, _bankRepo.Object, _currencyRepo.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_WhenBankIsMissing_ThrowsException()
    {
        var fund = new Fund { Id = Guid.NewGuid() };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };
        var command = new UpdateFundCommand
        {
            Id = fund.Id,
            BankId = Guid.NewGuid(),
            CurrencyId = currency.Id,
            Amount = 10m,
            TimeStamp = DateTime.UtcNow,
        };

        _fundRepo.Setup(r => r.GetByIdAsync(fund.Id, It.IsAny<CancellationToken>())).ReturnsAsync(fund);
        _currencyRepo.Setup(r => r.GetByIdAsync(currency.Id, It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _bankRepo.Setup(r => r.GetByIdAsync(command.BankId, It.IsAny<CancellationToken>())).ReturnsAsync((Bank?)null);

        var handler = new UpdateFundCommandHandler(_dbContext, _fundRepo.Object, _bankRepo.Object, _currencyRepo.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_WhenIdIsEmpty_ThrowsValidationException()
    {
        var command = new UpdateFundCommand
        {
            Id = Guid.Empty,
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            Amount = 10m,
            TimeStamp = DateTime.UtcNow,
        };

        var handler = new UpdateFundCommandHandler(_dbContext, _fundRepo.Object, _bankRepo.Object, _currencyRepo.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_WhenBankIdIsEmpty_ThrowsValidationException()
    {
        var command = new UpdateFundCommand
        {
            Id = Guid.NewGuid(),
            BankId = Guid.Empty,
            CurrencyId = Guid.NewGuid(),
            Amount = 10m,
            TimeStamp = DateTime.UtcNow,
        };

        var handler = new UpdateFundCommandHandler(_dbContext, _fundRepo.Object, _bankRepo.Object, _currencyRepo.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}