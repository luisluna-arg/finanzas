using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class DeleteCreditCardTransactionCommandHandler : BaseCommandHandler<DeleteCreditCardTransactionCommand, CreditCardTransaction>
{
    private readonly IRepository<CreditCardTransaction, Guid> transactionRepository;

    public DeleteCreditCardTransactionCommandHandler(
        FinanceDbContext db,
        IRepository<CreditCardTransaction, Guid> transactionRepository)
        : base(db)
    {
        this.transactionRepository = transactionRepository;
    }

    public override async Task<DataResult<CreditCardTransaction>> ExecuteAsync(DeleteCreditCardTransactionCommand command, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (transaction == null) throw new Exception("Credit card transaction not found");

        transaction.Deactivated = true;
        await transactionRepository.UpdateAsync(transaction, cancellationToken);

        return DataResult<CreditCardTransaction>.Success(transaction);
    }
}

public class DeleteCreditCardTransactionCommand : ICommand
{
    [Required]
    public Guid Id { get; set; }
}
