using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using FluentValidation;

namespace Finance.Application.Commands.Base;

public abstract class BatchUpdateBaseCommand : IContextAwareCommand<FinanceDispatchContext, CommandResult>
{
    internal FinanceDispatchContext Context { get; private set; } = new();
    public Guid[] Ids { get; set; } = [];
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public abstract class BatchUpdateBaseCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : BatchUpdateBaseCommand
{
    public BatchUpdateBaseCommandValidator() : base()
    {
        RuleFor(x => x.Ids)
            .NotEmpty()
            .WithMessage("At least one Id is required.");

        RuleForEach(x => x.Ids)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");
    }
}
