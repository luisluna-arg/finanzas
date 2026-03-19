using CQRSDispatch;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.Subscriptions.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;

namespace Finance.Application.Commands.Subscriptions;

public class CreateSubscriptionCommand : UpsertSubscriptionBaseCommand;

public class CreateSubscriptionCommandHandler(
    FinanceDbContext db,
    IRepository<Subscription, Guid> subscriptionRepository,
    IRepository<Currency, Guid> currencyRepository
) : BaseCommandHandler<CreateSubscriptionCommand, Subscription>(db)
{
    private readonly IRepository<Subscription, Guid> _subscriptionRepository = subscriptionRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository = currencyRepository;

    public override async Task<DataResult<Subscription>> ExecuteAsync(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new CreateSubscriptionCommandValidator());

        var currency = await _currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var subscription = new Subscription();
        subscription.Name = command.Name;
        subscription.Currency = currency;
        subscription.Price = command.Price;
        subscription.Frequency = command.Frequency;

        await _subscriptionRepository.AddAsync(subscription, cancellationToken);

        return DataResult<Subscription>.Success(subscription);
    }
}

public class CreateSubscriptionCommandValidator : UpsertSubscriptionBaseCommandValidator<CreateSubscriptionCommand>;
