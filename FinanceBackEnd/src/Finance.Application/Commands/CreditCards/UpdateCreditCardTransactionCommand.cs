using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class UpdateCreditCardTransactionCommandHandler(
    IRepository<CreditCardTransaction, Guid> transactionRepository,
    FinanceDbContext db)
    : BaseUpdateCommandHandler<CreditCardTransaction, Guid, UpdateCreditCardTransactionCommand>(
        transactionRepository, db)
{
    protected override Task<CreditCardTransaction> UpdateRecord(
        UpdateCreditCardTransactionCommand command,
        CreditCardTransaction record,
        CancellationToken cancellationToken)
    {
        record.Timestamp = command.Timestamp;
        record.Concept = command.Concept;
        record.Amount = command.Amount;
        record.CurrencyId = command.CurrencyId;
        return Task.FromResult(record);
    }
}

public class UpdateCreditCardTransactionCommand : BaseUpdateCommand<CreditCardTransaction, Guid>
{
    [Required]
    public DateTime Timestamp { get; set; }
    [Required]
    [StringLength(500)]
    public string Concept { get; set; } = string.Empty;
    [Required]
    public Money Amount { get; set; } = 0;

    public Guid CurrencyId { get; set; }
}

public class UpdateCreditCardTransactionCommandValidator
    : BaseUpdateCommandValidator<UpdateCreditCardTransactionCommand, CreditCardTransaction, Guid>
{
    public UpdateCreditCardTransactionCommandValidator(
        IRepository<CreditCardTransaction, Guid> repository)
        : base(repository)
    {
    }
}
