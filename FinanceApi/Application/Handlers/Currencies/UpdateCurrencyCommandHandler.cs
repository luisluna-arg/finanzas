using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Currencies;

public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, Currency>
{
    private readonly FinanceDbContext dbContext;

    public UpdateCurrencyCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Currency> Handle(UpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var currency = await GetCurrency(command.Id);

        currency.Name = command.Name;
        currency.ShortName = command.ShortName;

        await dbContext.SaveChangesAsync();

        return await Task.FromResult(currency);
    }

    private async Task<Currency> GetCurrency(Guid id)
    {
        var currency = await dbContext.Currency.FirstOrDefaultAsync(o => o.Id == id);
        if (currency == null) throw new Exception("Currency not found");
        return currency;
    }
}
