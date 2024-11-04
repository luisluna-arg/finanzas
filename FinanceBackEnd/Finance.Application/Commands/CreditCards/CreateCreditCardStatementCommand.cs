using System.ComponentModel.DataAnnotations;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using FluentValidation;

namespace Finance.Application.Commands.CreditCards;

public class CreateCreditCardStatementCommandHandler(
    IRepository<CreditCardStatement, Guid> creditCardStatementRepository,
    IRepository<CreditCard, Guid> bankRepository,
    FinanceDbContext db)
    : BaseCreateCommandHandler<CreditCardStatement, Guid, CreateCreditCardStatementCommand>(
        creditCardStatementRepository, db)
{
    private IRepository<CreditCard, Guid> BankRepository { get => bankRepository; }

    protected override async Task<CreditCardStatement> BuildRecord(CreateCreditCardStatementCommand command, CancellationToken cancellationToken)
    {
        var creditCard = await BankRepository.GetByIdAsync(command.CreditCardId, cancellationToken);
        if (creditCard == null) throw new Exception("CreditCard not found");

        var newCreditCardStatement = new CreditCardStatement()
        {
            CreditCard = creditCard,
            ClosureDate = command.ClosureDate,
            ExpiringDate = command.ExpiringDate,
            Deactivated = command.Deactivated ?? false
        };

        return newCreditCardStatement;
    }
}

public class CreateCreditCardStatementCommand : BaseCreateCommand<CreditCardStatement>
{
    [Required]
    public Guid CreditCardId { get; set; }

    [Required]
    public DateTime ClosureDate { get; set; }

    [Required]
    public DateTime ExpiringDate { get; set; }

    public bool? Deactivated { get; set; }
}

public class CreateCreditCardStatementCommandValidator
    : BaseCreateCommandValidator<CreateCreditCardStatementCommand, CreditCardStatement>
{
    public CreateCreditCardStatementCommandValidator(
        IRepository<CreditCard, Guid> creditCardRepository)
        : base()
    {
        RuleFor(c => c.CreditCardId)
            .MustAsync(creditCardRepository.ExistsAsync)
            .WithMessage($"{typeof(CreditCard).Name} must exist");
    }
}
