using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Incomes;
using Finance.Application.Repositories;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Incomes;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Incomes;

public class CreateIncomeCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<Income, Guid>> _incomeRepo;
    private readonly Mock<IRepository<Bank, Guid>> _bankRepo;
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepo;
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;

    public CreateIncomeCommandHandlerTests()
    {
        _incomeRepo = new Mock<IRepository<Income, Guid>>();
        _bankRepo = new Mock<IRepository<Bank, Guid>>();
        _currencyRepo = new Mock<IRepository<Currency, Guid>>();
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

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
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IncomePermissions>>(It.IsAny<CreateIncomePermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IncomePermissions>.Success(new IncomePermissions()));

        var handler = new CreateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object, _dispatcher.Object);
        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(bank, result.Data.Bank);
        Assert.Equal(currency, result.Data.Currency);
        Assert.Equal(100m, (decimal)result.Data.Amount);

        _incomeRepo.Verify(r => r.AddAsync(It.IsAny<Income>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IncomePermissions>>(
            It.Is<CreateIncomePermissionsCommand>(c =>
                c.ResourceId == result.Data.Id &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
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

        var handler = new CreateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object, _dispatcher.Object);
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

        var handler = new CreateIncomeCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _incomeRepo.Object, _dispatcher.Object);
        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }
}
