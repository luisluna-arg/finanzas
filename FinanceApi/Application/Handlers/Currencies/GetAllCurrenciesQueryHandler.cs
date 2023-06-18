using FinanceApi.Application.Models;
using FinanceApi.Application.Queries.Currencies;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Currencies;

public class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, Currency[]>
{
    private readonly FinanceDbContext dbContext;

    public GetAllCurrenciesQueryHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Currency[]> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await dbContext.Currency.ToArrayAsync());
    }
}
