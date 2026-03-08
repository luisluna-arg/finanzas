using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Domain.Models.Funds;
using Finance.Domain.SpecialTypes;
using FluentValidation;

namespace Finance.Application.Commands.Funds.Base;

public abstract class UpsertFundBaseCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<Fund>>
{
    public virtual Guid BankId { get; set; }
    public virtual Guid CurrencyId { get; set; }
    public DateTime TimeStamp { get; set; }
    public Money Amount { get; set; }
    public bool? DailyUse { get; set; }
    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public abstract class UpsertFundBaseCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : UpsertFundBaseCommand
{
    public UpsertFundBaseCommandValidator() : base()
    {
        RuleFor(x => x.BankId)
            .NotEmpty()
            .WithMessage("Bank Id is required.");

        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithMessage("Currency Id is required.");

        RuleFor(x => x.TimeStamp)
            .NotEmpty()
            .WithMessage("Timestamp is required.");
    }
}
