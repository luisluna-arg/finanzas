using System.ComponentModel.DataAnnotations;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using FluentValidation;

namespace FinanceApi.Application.Commands.CreditCards;

public class UpdateCreditCardStatementCommandHandler(
    IRepository<CreditCardStatement, Guid> creditCardStatementRepository,
    IRepository<CreditCard, Guid> creditCardRepository,
    FinanceDbContext db)
    : BaseUpdateCommandHandler<CreditCardStatement, Guid, UpdateCreditCardStatementCommand>(
        creditCardStatementRepository, db)
{
    private IRepository<CreditCard, Guid> CreditCardRepository { get => creditCardRepository; }

    protected override async Task<CreditCardStatement> UpdateRecord(UpdateCreditCardStatementCommand command, CreditCardStatement record, CancellationToken cancellationToken)
    {
        var creditCard = await CreditCardRepository.GetByIdAsync(command.CreditCardId, cancellationToken);
        if (creditCard == null) throw new Exception($"{typeof(CreditCard).Name} not found");

        record.CreditCard = creditCard;
        record.ClosureDate = command.ClosureDate;
        record.ExpiringDate = command.ExpiringDate;
        record.Deactivated = command.Deactivated ?? false;

        return record;
    }
}

public class UpdateCreditCardStatementCommand : BaseUpdateCommand<CreditCardStatement, Guid>
{
    [Required]
    public Guid CreditCardId { get; set; }

    [Required]
    public DateTime ClosureDate { get; set; }

    [Required]
    public DateTime ExpiringDate { get; set; }

    public bool? Deactivated { get; set; }
}

public class UpdateCreditCardStatementCommandValidator
    : BaseUpdateCommandValidator<UpdateCreditCardStatementCommand, CreditCardStatement, Guid>
{
    public UpdateCreditCardStatementCommandValidator(
        IRepository<CreditCardStatement, Guid> repository,
        IRepository<CreditCard, Guid> creditCardRepository)
        : base(repository)
    {
        RuleFor(c => c.CreditCardId)
            .MustAsync(creditCardRepository.ExistsAsync)
            .WithMessage($"{typeof(CreditCard).Name} must exist");
    }
}
