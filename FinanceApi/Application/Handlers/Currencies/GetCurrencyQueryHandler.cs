using FinanceApi.Application.Queries.Currencies;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Currencies;

public class GetCurrencyQueryHandler : IRequestHandler<GetCurrencyQuery, Currency>
{
    private readonly FinanceDbContext dbContext;

    public GetCurrencyQueryHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Currency> Handle(GetCurrencyQuery request, CancellationToken cancellationToken)
    {
        var module = await dbContext.Currency.FirstOrDefaultAsync(o => o.Id == request.Id);
        if (module == null) throw new Exception("Currency not found");
        return await Task.FromResult(module);
    }
}
