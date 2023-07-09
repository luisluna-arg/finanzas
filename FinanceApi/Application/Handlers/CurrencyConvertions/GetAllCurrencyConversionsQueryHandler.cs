using FinanceApi.Application.Queries.CurrencyConversions;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.CurrencyConvertions;

public class GetAllCurrencyConversionsQueryHandler : BaseCollectionHandler<GetAllCurrencyConversionsQuery, CurrencyConversion>
{
    public GetAllCurrencyConversionsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CurrencyConversion>> Handle(GetAllCurrencyConversionsQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await DbContext.CurrencyConversion.ToArrayAsync());
    }
}
