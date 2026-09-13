using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.BankCurrencies;

public class DeleteBankCurrencyCommand : ICommand
{
    public Guid BankId { get; set; }
    public Guid CurrencyId { get; set; }
}

public class DeleteBankCurrencyCommandHandler(FinanceDbContext db) : ICommandHandler<DeleteBankCurrencyCommand>
{
    public async Task<CommandResult> ExecuteAsync(DeleteBankCurrencyCommand command, CancellationToken cancellationToken = default)
    {
        var bankCurrency = await db.BankCurrency
            .FirstOrDefaultAsync(o => o.BankId == command.BankId && o.CurrencyId == command.CurrencyId, cancellationToken);

        if (bankCurrency == null)
        {
            return CommandResult.Failure("Bank/currency combination not found");
        }

        var permissions = await db.BankCurrencyPermissions
            .Where(p => p.BankId == command.BankId && p.CurrencyId == command.CurrencyId)
            .ToListAsync(cancellationToken);

        db.BankCurrencyPermissions.RemoveRange(permissions);
        db.BankCurrency.Remove(bankCurrency);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Failure("Bank/currency combination is still in use by existing funds");
        }

        return CommandResult.Success();
    }
}
