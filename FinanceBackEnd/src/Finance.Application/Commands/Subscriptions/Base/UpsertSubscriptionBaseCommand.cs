using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Domain.Enums;
using Finance.Domain.Models.Subscriptions;
using FluentValidation;

namespace Finance.Application.Commands.Subscriptions.Base;

public abstract class UpsertSubscriptionBaseCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<Subscription>>
{
    internal Guid UserId { get; set; }
    internal FinanceDispatchContext Context { get; private set; } = new();
    [Required]
    public Guid CurrencyId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }
    public FrequencyEnum Frequency { get; set; } = FrequencyEnum.Monthly;

    public void SetContext(FinanceDispatchContext context)
    {
        Context = context;
    }
}

public abstract class UpsertSubscriptionBaseCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : UpsertSubscriptionBaseCommand
{
    public UpsertSubscriptionBaseCommandValidator() : base()
    {
        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithMessage("Currency Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Name is required.")
            .MaximumLength(200)
                .WithMessage("Name must be at most 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be greater than or equal to 0.");
    }
}
