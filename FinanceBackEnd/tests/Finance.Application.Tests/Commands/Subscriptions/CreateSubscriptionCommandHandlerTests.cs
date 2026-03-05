using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.Subscriptions;
using Finance.Application.Legacy.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Subscriptions;

public class CreateSubscriptionCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<Subscription, Guid>> _subscriptionRepo;
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepo;
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;

    public CreateSubscriptionCommandHandlerTests()
    {
        _subscriptionRepo = new Mock<IRepository<Subscription, Guid>>();
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
    public async Task Create_HappyPath_AddsCurrencyAndReturnsSuccess()
    {
        var currency = new Currency { Id = Guid.NewGuid(), Name = "", ShortName = "" };
        var command = new CreateSubscriptionCommand
        {
            CurrencyId = currency.Id,
            Name = "Netflix",
            Price = 9.99m,
            Frequency = FrequencyEnum.Monthly,
        };

        _currencyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _dispatcher.Setup(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(
            It.IsAny<CreateSubscriptionOwnershipCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<SubscriptionPermissions>.Success(new SubscriptionPermissions()));

        var handler = new CreateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object, _dispatcher.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal("Netflix", result.Data.Name);
        Assert.Equal(9.99m, (decimal)result.Data.Price);
        Assert.Equal(currency, result.Data.Currency);
        Assert.Equal(FrequencyEnum.Monthly, result.Data.Frequency);
    }

    [Fact]
    public async Task Create_HappyPath_AddsSubscriptionToRepository()
    {
        var currency = new Currency { Id = Guid.NewGuid(), Name = "", ShortName = "" };
        var command = new CreateSubscriptionCommand { CurrencyId = currency.Id, Name = "Netflix", Price = 9.99m };

        _currencyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _dispatcher.Setup(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(
            It.IsAny<CreateSubscriptionOwnershipCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<SubscriptionPermissions>.Success(new SubscriptionPermissions()));

        var handler = new CreateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object, _dispatcher.Object);

        await handler.ExecuteAsync(command, default);

        _subscriptionRepo.Verify(r => r.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Create_HappyPath_DispatchesOwnershipCommandWithOwnerPermission()
    {
        var currency = new Currency { Id = Guid.NewGuid(), Name = "", ShortName = "" };
        var command = new CreateSubscriptionCommand { CurrencyId = currency.Id, Name = "Netflix", Price = 9.99m };

        _currencyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _dispatcher.Setup(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(
            It.IsAny<CreateSubscriptionOwnershipCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<SubscriptionPermissions>.Success(new SubscriptionPermissions()));

        var handler = new CreateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object, _dispatcher.Object);

        await handler.ExecuteAsync(command, default);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(
            It.Is<CreateSubscriptionOwnershipCommand>(c =>
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_CurrencyNotFound_ThrowsException()
    {
        _currencyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);
        var command = new CreateSubscriptionCommand { CurrencyId = Guid.NewGuid(), Name = "Netflix", Price = 9.99m };

        var handler = new CreateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object, _dispatcher.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_EmptyName_ThrowsValidationException()
    {
        var command = new CreateSubscriptionCommand { CurrencyId = Guid.NewGuid(), Name = "", Price = 9.99m };

        var handler = new CreateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object, _dispatcher.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_EmptyCurrencyId_ThrowsValidationException()
    {
        var command = new CreateSubscriptionCommand { CurrencyId = Guid.Empty, Name = "Netflix", Price = 9.99m };

        var handler = new CreateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object, _dispatcher.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_NegativePrice_ThrowsValidationException()
    {
        var command = new CreateSubscriptionCommand { CurrencyId = Guid.NewGuid(), Name = "Netflix", Price = -1m };

        var handler = new CreateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object, _dispatcher.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}
