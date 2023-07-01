using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.AppModules;

public class GetAllAppModulesQueryHandler : IRequestHandler<GetAllAppModulesQuery, AppModule[]>
{
    private readonly FinanceDbContext dbContext;

    public GetAllAppModulesQueryHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<AppModule[]> Handle(GetAllAppModulesQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await dbContext.AppModule.Include(o => o.Currency).ToArrayAsync());
    }
}
