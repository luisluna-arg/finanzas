using Finance.Application.Legacy.Commands.Subscriptions;
using Finance.Application.Legacy.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using Finance.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Subscriptions;

public class UpdateSubscriptionCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<Subscription, Guid>> _subscriptionRepo;
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepo;
    private readonly FinanceDbContext _dbContext;

    public UpdateSubscriptionCommandHandlerTests()
    {
        _subscriptionRepo = new Mock<IRepository<Subscription, Guid>>();
        _currencyRepo = new Mock<IRepository<Currency, Guid>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task Update_HappyPath_UpdatesSubscriptionAndReturnsSuccess()
    {
        var subscriptionId = Guid.NewGuid();
        var currency = new Currency { Id = Guid.NewGuid(), Name = "", ShortName = "" };
        var subscription = new Subscription { Id = subscriptionId, Name = "Old", Price = 5m };
        var command = new UpdateSubscriptionCommand
        {
            Id = subscriptionId,
            CurrencyId = currency.Id,
            Name = "Updated",
            Price = 14.99m,
            Frequency = FrequencyEnum.Annual,
        };

        _currencyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _subscriptionRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(subscription);

        var handler = new UpdateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", result.Data.Name);
        Assert.Equal(14.99m, (decimal)result.Data.Price);
        Assert.Equal(currency, result.Data.Currency);
        Assert.Equal(FrequencyEnum.Annual, result.Data.Frequency);
    }

    [Fact]
    public async Task Update_HappyPath_CallsUpdateOnRepository()
    {
        var subscription = new Subscription { Id = Guid.NewGuid(), Name = "Old" };
        var currency = new Currency { Id = Guid.NewGuid(), Name = "", ShortName = "" };
        var command = new UpdateSubscriptionCommand { Id = subscription.Id, CurrencyId = currency.Id, Name = "Updated", Price = 9.99m };

        _currencyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _subscriptionRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(subscription);

        var handler = new UpdateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object);

        await handler.ExecuteAsync(command, default);

        _subscriptionRepo.Verify(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Update_CurrencyNotFound_ThrowsException()
    {
        _currencyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);
        var command = new UpdateSubscriptionCommand { Id = Guid.NewGuid(), CurrencyId = Guid.NewGuid(), Name = "Netflix", Price = 9.99m };

        var handler = new UpdateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_SubscriptionNotFound_ThrowsException()
    {
        var currency = new Currency { Id = Guid.NewGuid(), Name = "", ShortName = "" };
        _currencyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _subscriptionRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Subscription?)null);

        var command = new UpdateSubscriptionCommand { Id = Guid.NewGuid(), CurrencyId = currency.Id, Name = "Netflix", Price = 9.99m };

        var handler = new UpdateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_EmptyId_ThrowsValidationException()
    {
        var command = new UpdateSubscriptionCommand { Id = Guid.Empty, CurrencyId = Guid.NewGuid(), Name = "Netflix", Price = 9.99m };

        var handler = new UpdateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_EmptyName_ThrowsValidationException()
    {
        var command = new UpdateSubscriptionCommand { Id = Guid.NewGuid(), CurrencyId = Guid.NewGuid(), Name = "", Price = 9.99m };

        var handler = new UpdateSubscriptionCommandHandler(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}
