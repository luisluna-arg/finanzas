using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using FluentValidation;

namespace Finance.Application.Commands.Base;

public abstract class BatchUpdateBaseCommand<TId> : IContextAwareCommand<FinanceDispatchContext, CommandResult>
{
    internal FinanceDispatchContext Context { get; private set; } = new();
    public TId[] Ids { get; init; } = [];
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public abstract class BatchUpdateBaseCommand : BatchUpdateBaseCommand<Guid>
{
}

public abstract class BatchUpdateBaseCommandValidator<TCommand, TId> : AbstractValidator<TCommand>
    where TCommand : BatchUpdateBaseCommand<TId>
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

public abstract class BatchUpdateBaseCommandValidator<TCommand> : BatchUpdateBaseCommandValidator<TCommand, Guid>
    where TCommand : BatchUpdateBaseCommand
{
    public BatchUpdateBaseCommandValidator() : base()
    {
    }
}
