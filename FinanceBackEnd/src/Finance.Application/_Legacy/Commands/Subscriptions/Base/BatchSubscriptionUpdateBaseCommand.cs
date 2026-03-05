using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using FluentValidation;

namespace Finance.Application.Legacy.Commands.Subscriptions.Base;

public abstract class BatchSubscriptionUpdateBaseCommand : IContextAwareCommand<FinanceDispatchContext, CommandResult>
{
    internal Guid UserId { get; set; }
    internal FinanceDispatchContext Context { get; private set; } = new();
    public Guid[] Ids { get; set; } = [];

    public void SetContext(FinanceDispatchContext context)
    {
        Context = context;
    }
}


public abstract class BatchSubscriptionUpdateBaseCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : BatchSubscriptionUpdateBaseCommand
{
    public BatchSubscriptionUpdateBaseCommandValidator() : base()
    {
        RuleFor(x => x.Ids)
            .NotEmpty()
            .WithMessage("At least one Subscription Id is required.");

        RuleForEach(x => x.Ids)
            .NotEmpty()
            .WithMessage("Subscription Id cannot be empty.");
    }
}
