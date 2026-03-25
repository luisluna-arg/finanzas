using Finance.Application.Commands.Subscriptions;
using Finance.Application.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Subscriptions;
using FluentValidation;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Commands.Subscriptions;

public class CreateSubscriptionCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<Subscription, Guid>> _subscriptionRepo;
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepo;

    public CreateSubscriptionCommandHandlerTests()
    {
        _subscriptionRepo = new Mock<IRepository<Subscription, Guid>>();
        _currencyRepo = new Mock<IRepository<Currency, Guid>>();
    }

    private CreateSubscriptionCommandHandler CreateHandler() =>
        new(_dbContext, _subscriptionRepo.Object, _currencyRepo.Object);

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

        var result = await CreateHandler().ExecuteAsync(command, default);

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

        await CreateHandler().ExecuteAsync(command, default);

        _subscriptionRepo.Verify(r => r.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Create_CurrencyNotFound_ThrowsException()
    {
        _currencyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Currency?)null);
        var command = new CreateSubscriptionCommand { CurrencyId = Guid.NewGuid(), Name = "Netflix", Price = 9.99m };

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_EmptyName_ThrowsValidationException()
    {
        var command = new CreateSubscriptionCommand { CurrencyId = Guid.NewGuid(), Name = "", Price = 9.99m };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_EmptyCurrencyId_ThrowsValidationException()
    {
        var command = new CreateSubscriptionCommand { CurrencyId = Guid.Empty, Name = "Netflix", Price = 9.99m };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_NegativePrice_ThrowsValidationException()
    {
        var command = new CreateSubscriptionCommand { CurrencyId = Guid.NewGuid(), Name = "Netflix", Price = -1m };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }
}
