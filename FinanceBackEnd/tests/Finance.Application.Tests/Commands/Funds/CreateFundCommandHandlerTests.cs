using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Funds;
using Finance.Application.Repositories;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Funds;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Funds;

public class CreateFundCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<Fund, Guid>> _fundRepo;
    private readonly Mock<IRepository<Bank, Guid>> _bankRepo;
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepo;
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;

    public CreateFundCommandHandlerTests()
    {
        _fundRepo = new Mock<IRepository<Fund, Guid>>();
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
    public async Task Create_HappyPath_AddsFundAndReturnsSuccess()
    {
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Santander" };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };
        var command = new CreateFundCommand
        {
            BankId = bank.Id,
            CurrencyId = currency.Id,
            TimeStamp = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            Amount = 100m,
            DailyUse = true,
        };

        _bankRepo.Setup(r => r.GetByIdAsync(bank.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bank);
        _currencyRepo.Setup(r => r.GetByIdAsync(currency.Id, It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _fundRepo
            .Setup(r => r.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .Callback<Fund, CancellationToken, bool>((fund, _, _) => fund.Id = Guid.NewGuid())
            .Returns(Task.CompletedTask);
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<FundPermissions>>(It.IsAny<CreateFundPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<FundPermissions>.Success(new FundPermissions()));

        var handler = new CreateFundCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _fundRepo.Object, _dispatcher.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(bank, result.Data.Bank);
        Assert.Equal(currency, result.Data.Currency);
        Assert.Equal(100m, (decimal)result.Data.Amount);
        Assert.True(result.Data.DailyUse);

        _fundRepo.Verify(r => r.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync<DataResult<FundPermissions>>(
            It.Is<CreateFundPermissionsCommand>(c =>
                c.ResourceId == result.Data.Id &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenBankIsMissing_ThrowsException()
    {
        var command = new CreateFundCommand
        {
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            TimeStamp = DateTime.UtcNow,
            Amount = 100m,
        };

        _bankRepo.Setup(r => r.GetByIdAsync(command.BankId, It.IsAny<CancellationToken>())).ReturnsAsync((Bank?)null);

        var handler = new CreateFundCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _fundRepo.Object, _dispatcher.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_WhenCurrencyIsMissing_ThrowsException()
    {
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Santander" };
        var command = new CreateFundCommand
        {
            BankId = bank.Id,
            CurrencyId = Guid.NewGuid(),
            TimeStamp = DateTime.UtcNow,
            Amount = 100m,
        };

        _bankRepo.Setup(r => r.GetByIdAsync(bank.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bank);
        _currencyRepo.Setup(r => r.GetByIdAsync(command.CurrencyId, It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);

        var handler = new CreateFundCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _fundRepo.Object, _dispatcher.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_WhenBankIdIsEmpty_ThrowsValidationException()
    {
        var command = new CreateFundCommand
        {
            BankId = Guid.Empty,
            CurrencyId = Guid.NewGuid(),
            TimeStamp = DateTime.UtcNow,
            Amount = 100m,
        };

        var handler = new CreateFundCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _fundRepo.Object, _dispatcher.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_WhenCurrencyIdIsEmpty_ThrowsValidationException()
    {
        var command = new CreateFundCommand
        {
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.Empty,
            TimeStamp = DateTime.UtcNow,
            Amount = 100m,
        };

        var handler = new CreateFundCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _fundRepo.Object, _dispatcher.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_WhenTimestampIsDefault_ThrowsValidationException()
    {
        var command = new CreateFundCommand
        {
            BankId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
            TimeStamp = default,
            Amount = 100m,
        };

        var handler = new CreateFundCommandHandler(_dbContext, _bankRepo.Object, _currencyRepo.Object, _fundRepo.Object, _dispatcher.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}