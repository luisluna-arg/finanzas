using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class CreateCreditCardTransactionCommandHandler : BaseCommandHandler<CreateCreditCardTransactionCommand, CreditCardTransaction>
{
    private readonly IRepository<CreditCardTransaction, Guid> transactionRepository;
    private readonly IRepository<CreditCard, Guid> creditCardRepository;

    public CreateCreditCardTransactionCommandHandler(
        FinanceDbContext db,
        IRepository<CreditCard, Guid> creditCardRepository,
        IRepository<CreditCardTransaction, Guid> transactionRepository)
        : base(db)
    {
        this.creditCardRepository = creditCardRepository;
        this.transactionRepository = transactionRepository;
    }

    public override async Task<DataResult<CreditCardTransaction>> ExecuteAsync(CreateCreditCardTransactionCommand command, CancellationToken cancellationToken)
    {
        var creditCard = await creditCardRepository.GetByIdAsync(command.CreditCardId, cancellationToken);
        if (creditCard == null) throw new Exception("Credit card not found");

        var newTransaction = new CreditCardTransaction()
        {
            CreditCardId = command.CreditCardId,
            Timestamp = command.Timestamp,
            TransactionType = command.TransactionType,
            Concept = command.Concept,
            Amount = command.Amount,
            Reference = command.Reference,
            Deactivated = command.Deactivated
        };

        await transactionRepository.AddAsync(newTransaction, cancellationToken);

        return DataResult<CreditCardTransaction>.Success(newTransaction);
    }
}

public class CreateCreditCardTransactionCommand : ICommand
{
    [Required]
    public Guid CreditCardId { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Required]
    public CreditCardTransactionType TransactionType { get; set; }

    [Required]
    [StringLength(500)]
    public string Concept { get; set; } = string.Empty;

    [Required]
    public Money Amount { get; set; } = 0;

    [StringLength(200)]
    public string? Reference { get; set; }

    public bool Deactivated { get; set; } = false;
}
