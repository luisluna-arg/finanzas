using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Domain.Enums;
using Finance.Domain.Models.Debits;
using Finance.Domain.SpecialTypes;
using FluentValidation;

namespace Finance.Application.Commands.Debits.Base;

public abstract class UpsertDebitBaseCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<Debit>>
{
    [Required]
    public Guid AppModuleId { get; set; }
    [Required]
    public string Origin { get; set; } = string.Empty;
    [Required]
    public Money Amount { get; set; } = 0m;
    public bool Deactivated { get; set; }
    public FrequencyEnum Frequency { get; set; } = FrequencyEnum.Monthly;

    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public abstract class UpsertDebitBaseCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : UpsertDebitBaseCommand
{
    public UpsertDebitBaseCommandValidator() : base()
    {
        RuleFor(x => x.AppModuleId)
            .NotEmpty()
            .WithMessage("App Module Id is required.");

        RuleFor(x => x.Origin)
            .NotEmpty()
                .WithMessage("Origin is required.")
            .MaximumLength(200)
                .WithMessage("Origin must be at most 200 characters.");

        RuleFor(x => x.Amount)
            .Must(a => (decimal)a > 0)
            .WithMessage("Amount must be greater than 0.");
    }
}
