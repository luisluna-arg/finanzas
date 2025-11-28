using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Commands.Subscriptions.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using FluentValidation;

namespace Finance.Application.Commands.Subscriptions;

public class UpdateSubscriptionCommand : UpsertSubscriptionBaseCommand
{
    [Required]
    public Guid Id { get; set; }
}

public class UpdateSubscriptionCommandHandler(
    FinanceDbContext dbContext,
    IRepository<Subscription, Guid> subscriptionRepository,
    IRepository<Currency, Guid> currencyRepository
) : BaseCommandHandler<UpdateSubscriptionCommand, Subscription>(dbContext)
{
    private readonly IRepository<Subscription, Guid> _subscriptionRepository = subscriptionRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository = currencyRepository;

    public override async Task<DataResult<Subscription>> ExecuteAsync(UpdateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new UpdateSubscriptionCommandValidator());

        var currency = await _currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var subscription = await _subscriptionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (subscription == null) throw new Exception("Subscription not found");

        subscription.Name = command.Name;
        subscription.Currency = currency;
        subscription.Price = command.Price;
        subscription.Frequency = command.Frequency;

        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);

        return DataResult<Subscription>.Success(subscription);
    }
}

public class UpdateSubscriptionCommandValidator : UpsertSubscriptionBaseCommandValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator() : base()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Subscription Id is required.");
    }
}