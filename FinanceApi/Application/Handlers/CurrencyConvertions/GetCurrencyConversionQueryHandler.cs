using FinanceApi.Application.Queries.CurrencyConversions;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.CurrencyConvertions;

public class GetCurrencyConversionQueryHandler : BaseResponseHandler<GetCurrencyConversionQuery, CurrencyConversion>
{
    public GetCurrencyConversionQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<CurrencyConversion> Handle(GetCurrencyConversionQuery request, CancellationToken cancellationToken)
    {
        var currencyConversion = await DbContext.CurrencyConversion.FirstOrDefaultAsync(o => o.Id == request.Id);

        if (currencyConversion == null) throw new Exception("Currency Conversion not found");

        return await Task.FromResult(currencyConversion);
    }
}
