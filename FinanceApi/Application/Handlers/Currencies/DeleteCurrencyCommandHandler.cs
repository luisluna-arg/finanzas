using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Currencies;

public class DeleteCurrencyCommandHandler : BaseResponselessHandler<DeleteCurrencyCommand>
{
    public DeleteCurrencyCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task Handle(DeleteCurrencyCommand command, CancellationToken cancellationToken)
    {
        var currency = await GetCurrency(command.Id);
        DbContext.Remove(currency);
        await DbContext.SaveChangesAsync();
    }

    private async Task<Currency> GetCurrency(Guid id)
    {
        var currency = await DbContext.Currency.FirstOrDefaultAsync(o => o.Id == id);
        if (currency == null) throw new Exception("Currency not found");
        return currency;
    }
}
