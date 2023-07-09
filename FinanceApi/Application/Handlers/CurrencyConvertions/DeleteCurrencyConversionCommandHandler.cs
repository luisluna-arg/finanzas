using FinanceApi.Application.Commands.CurrencyConversions;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.CurrencyConvertions;

public class DeleteCurrencyConversionCommandHandler : BaseResponselessHandler<DeleteCurrencyConversionCommand>
{
    public DeleteCurrencyConversionCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task Handle(DeleteCurrencyConversionCommand command, CancellationToken cancellationToken)
    {
        var currency = await GetCurrencyConversion(command.Id);
        DbContext.Remove(currency);
        await DbContext.SaveChangesAsync();
    }

    private async Task<CurrencyConversion> GetCurrencyConversion(Guid id)
    {
        var currencyConversion = await DbContext.CurrencyConversion.FirstOrDefaultAsync(o => o.Id == id);
        if (currencyConversion == null) throw new Exception("Currency Conversion not found");
        return currencyConversion;
    }
}
