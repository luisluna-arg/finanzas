using FinanceApi.Application.Models;
using FinanceApi.Application.Queries.Modules;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Commands.Modules;

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
