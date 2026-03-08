using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Base.Handlers;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.Subscriptions.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;

namespace Finance.Application.Commands.Subscriptions;

public class CreateSubscriptionCommand : UpsertSubscriptionBaseCommand;

public class CreateSubscriptionCommandHandler(
    FinanceDbContext db,
    IRepository<Subscription, Guid> subscriptionRepository,
    IRepository<Currency, Guid> currencyRepository,
    IDispatcher<FinanceDispatchContext> dispatcher
) : BaseCommandHandler<CreateSubscriptionCommand, Subscription>(db)
{
    private readonly IRepository<Subscription, Guid> _subscriptionRepository = subscriptionRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository = currencyRepository;
    private readonly IDispatcher<FinanceDispatchContext> _dispatcher = dispatcher;

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

        var ownershipCommand = new CreateSubscriptionOwnershipCommand
        {
            ResourceId = subscription.Id,
            PermissionLevels = [PermissionLevelEnum.Owner]
        };

        await _dispatcher.DispatchAsync(ownershipCommand, command.Context.HttpRequest);

        return DataResult<Subscription>.Success(subscription);
    }
}

public class CreateSubscriptionCommandValidator : UpsertSubscriptionBaseCommandValidator<CreateSubscriptionCommand>;
