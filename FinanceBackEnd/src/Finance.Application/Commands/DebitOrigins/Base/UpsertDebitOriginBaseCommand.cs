using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Domain.Models.Debits;
using FluentValidation;

namespace Finance.Application.Commands.DebitOrigins.Base;

public abstract class UpsertDebitOriginBaseCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<DebitOrigin>>
{
    [Required]
    public Guid AppModuleId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public bool Deactivated { get; set; }

    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public abstract class UpsertDebitOriginBaseCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : UpsertDebitOriginBaseCommand
{
    public UpsertDebitOriginBaseCommandValidator() : base()
    {
        RuleFor(x => x.AppModuleId)
            .NotEmpty()
            .WithMessage("App Module Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Name is required.")
            .MaximumLength(200)
                .WithMessage("Name must be at most 200 characters.");
    }
}
