using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Currencies;

public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand>
{
    private readonly FinanceDbContext dbContext;

    public DeleteCurrencyCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task Handle(DeleteCurrencyCommand command, CancellationToken cancellationToken)
    {
        var currency = await GetCurrency(command.Id);
        dbContext.Remove(currency);
        await dbContext.SaveChangesAsync();
    }

    private async Task<Currency> GetCurrency(Guid id)
    {
        var currency = await dbContext.Currency.FirstOrDefaultAsync(o => o.Id == id);
        if (currency == null) throw new Exception("Currency not found");
        return currency;
    }
}
