using Finance.Application.Commands.Incomes;
using Finance.Application.Repositories;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Incomes;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Incomes;

public class UpdateIncomeCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<Income, Guid>> _incomeRepo;
    private readonly Mock<IRepository<Bank, Guid>> _bankRepo;
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepo;
    private readonly FinanceDbContext _dbContext;

    public UpdateIncomeCommandHandlerTests()
    {
        _incomeRepo = new Mock<IRepository<Income, Guid>>();
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
    public async Task Update_HappyPath_UpdatesIncomeAndReturnsSuccess()
    {
        var bank = new Bank { Id = Guid.NewGuid(), Name = "BBVA" };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };
        var income = new Income
        {
            Id = Guid.NewGuid(),
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            Amount = new Money(10m),
            TimeStamp = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        var command = new UpdateIncomeCommand
        {
            Id = income.Id,
            BankId = bank.Id,
            CurrencyId = currency.Id,
            Amount = new Money(55m),
            TimeStamp = new DateTime(2025, 2, 2, 0, 0, 0, DateTimeKind.Utc),
        };

        _incomeRepo.Setup(r => r.GetByIdAsync(income.Id, It.IsAny<CancellationToken>())).ReturnsAsync(income);
        _bankRepo.Setup(r => r.GetByIdAsync(bank.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bank);
        _currencyRepo.Setup(r => r.GetByIdAsync(currency.Id, It.IsAny<CancellationToken>())).ReturnsAsync(currency);

        var handler = new UpdateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object);
        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(bank, result.Data.Bank);
        Assert.Equal(currency, result.Data.Currency);
        Assert.Equal(55m, (decimal)result.Data.Amount);
        Assert.Equal(command.TimeStamp, result.Data.TimeStamp);

        _incomeRepo.Verify(r => r.UpdateAsync(income, It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Update_WhenIncomeMissing_ThrowsException()
    {
        var command = new UpdateIncomeCommand
        {
            Id = Guid.NewGuid(),
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            Amount = new Money(10m),
            TimeStamp = DateTime.UtcNow,
        };

        _incomeRepo.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Income?)null);

        var handler = new UpdateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object);
        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_WhenCurrencyMissing_ThrowsException()
    {
        var income = new Income { Id = Guid.NewGuid() };
        var command = new UpdateIncomeCommand
        {
            Id = income.Id,
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            Amount = new Money(10m),
            TimeStamp = DateTime.UtcNow,
        };

        _incomeRepo.Setup(r => r.GetByIdAsync(income.Id, It.IsAny<CancellationToken>())).ReturnsAsync(income);
        _currencyRepo.Setup(r => r.GetByIdAsync(command.CurrencyId, It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);

        var handler = new UpdateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object);
        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_WhenBankMissing_ThrowsException()
    {
        var income = new Income { Id = Guid.NewGuid() };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };
        var command = new UpdateIncomeCommand
        {
            Id = income.Id,
            BankId = Guid.NewGuid(),
            CurrencyId = currency.Id,
            Amount = new Money(10m),
            TimeStamp = DateTime.UtcNow,
        };

        _incomeRepo.Setup(r => r.GetByIdAsync(income.Id, It.IsAny<CancellationToken>())).ReturnsAsync(income);
        _currencyRepo.Setup(r => r.GetByIdAsync(currency.Id, It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _bankRepo.Setup(r => r.GetByIdAsync(command.BankId, It.IsAny<CancellationToken>())).ReturnsAsync((Bank?)null);

        var handler = new UpdateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object);
        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }
}
