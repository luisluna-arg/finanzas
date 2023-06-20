using FinanceApi.Application.Queries.Modules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Modules;

public class GetAllModulesQueryHandler : IRequestHandler<GetAllModulesQuery, Module[]>
{
    private readonly FinanceDbContext dbContext;

    public GetAllModulesQueryHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Module[]> Handle(GetAllModulesQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await dbContext.Module.Include(o => o.Currency).ToArrayAsync());
    }
}
