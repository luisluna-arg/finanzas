using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.CreditCards;

public sealed class DeleteCreditCardStatementCommand : BaseDeleteCommand<Guid>;

public sealed class DeleteCreditCardStatementCommandHandler(FinanceDbContext db)
    : ICommandHandler<DeleteCreditCardStatementCommand>
{
    public async Task<CommandResult> ExecuteAsync(DeleteCreditCardStatementCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var id in command.Ids)
        {
            var statement = await db.CreditCardStatement
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (statement == null) continue;

            var linkedTransactionIds = await db.CreditCardStatementTransaction
                .Where(st => st.StatementId == id && st.CreditCardTransactionId != null)
                .Select(st => st.CreditCardTransactionId!.Value)
                .ToListAsync(cancellationToken);

            await db.CreditCardTransaction
                .Where(t => linkedTransactionIds.Contains(t.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.Deactivated, true), cancellationToken);

            await db.CreditCardPayment
                .Where(p => p.StatementId == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Deactivated, true), cancellationToken);

            statement.Deactivated = true;
        }

        await db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success();
    }
}

public sealed class DeleteCreditCardStatementCommandValidator(IRepository<CreditCardStatement, Guid> repository)
    : BaseDeleteCommandValidator<DeleteCreditCardStatementCommand, CreditCardStatement, Guid>(repository);
