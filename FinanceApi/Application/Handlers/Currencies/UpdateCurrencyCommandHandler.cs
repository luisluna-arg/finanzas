using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Currencies;

public class UpdateCurrencyCommandHandler : BaseResponseHandler<UpdateCurrencyCommand, Currency>
{
    public UpdateCurrencyCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Currency> Handle(UpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var currency = await GetCurrency(command.Id);

        currency.Name = command.Name;
        currency.ShortName = command.ShortName;

        await DbContext.SaveChangesAsync();

        return await Task.FromResult(currency);
    }

    private async Task<Currency> GetCurrency(Guid id)
    {
        var currency = await DbContext.Currency.FirstOrDefaultAsync(o => o.Id == id);
        if (currency == null) throw new Exception("Currency not found");
        return currency;
    }
}
