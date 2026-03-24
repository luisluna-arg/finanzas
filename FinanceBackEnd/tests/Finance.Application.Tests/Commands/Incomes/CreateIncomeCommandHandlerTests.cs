using Finance.Application.Commands.Incomes;
using Finance.Application.Repositories;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Incomes;
using Finance.Domain.SpecialTypes;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Commands.Incomes;

public class CreateIncomeCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<Income, Guid>> _incomeRepo;
    private readonly Mock<IRepository<Bank, Guid>> _bankRepo;
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepo;

    public CreateIncomeCommandHandlerTests()
    {
        _incomeRepo = new Mock<IRepository<Income, Guid>>();
        _bankRepo = new Mock<IRepository<Bank, Guid>>();
        _currencyRepo = new Mock<IRepository<Currency, Guid>>();
    }

    [Fact]
    public async Task Create_HappyPath_AddsIncomeAndReturnsSuccess()
    {
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Santander" };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };
        var command = new CreateIncomeCommand
        {
            BankId = bank.Id,
            CurrencyId = currency.Id,
            TimeStamp = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            Amount = new Money(100m),
        };

        _bankRepo.Setup(r => r.GetByIdAsync(bank.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bank);
        _currencyRepo.Setup(r => r.GetByIdAsync(currency.Id, It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _incomeRepo
            .Setup(r => r.AddAsync(It.IsAny<Income>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .Callback<Income, CancellationToken, bool>((income, _, _) => income.Id = Guid.NewGuid())
            .Returns(Task.CompletedTask);

        var handler = new CreateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object);
        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(bank, result.Data.Bank);
        Assert.Equal(currency, result.Data.Currency);
        Assert.Equal(100m, (decimal)result.Data.Amount);

        _incomeRepo.Verify(r => r.AddAsync(It.IsAny<Income>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Create_WhenBankIsMissing_ThrowsException()
    {
        var command = new CreateIncomeCommand
        {
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            TimeStamp = DateTime.UtcNow,
            Amount = new Money(100m),
        };

        _bankRepo.Setup(r => r.GetByIdAsync(command.BankId, It.IsAny<CancellationToken>())).ReturnsAsync((Bank?)null);

        var handler = new CreateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object);
        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_WhenCurrencyIsMissing_ThrowsException()
    {
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Santander" };
        var command = new CreateIncomeCommand
        {
            BankId = bank.Id,
            CurrencyId = Guid.NewGuid(),
            TimeStamp = DateTime.UtcNow,
            Amount = new Money(100m),
        };

        _bankRepo.Setup(r => r.GetByIdAsync(bank.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bank);
        _currencyRepo.Setup(r => r.GetByIdAsync(command.CurrencyId, It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);

        var handler = new CreateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object);
        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }
}
