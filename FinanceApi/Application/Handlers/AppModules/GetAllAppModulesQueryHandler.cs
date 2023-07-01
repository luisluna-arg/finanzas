using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.AppModules;

public class GetAllAppModulesQueryHandler : BaseCollectionHandler<GetAllAppModulesQuery, AppModule>
{
    public GetAllAppModulesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<AppModule>> Handle(GetAllAppModulesQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await DbContext.AppModule.Include(o => o.Currency).ToArrayAsync());
    }
}
