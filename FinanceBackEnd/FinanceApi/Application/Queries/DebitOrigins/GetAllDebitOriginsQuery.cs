using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.DebitOrigins;

public class GetAllDebitOriginsQueryHandler : BaseCollectionHandler<GetAllDebitOriginsQuery, DebitOrigin?>
{
    public GetAllDebitOriginsQueryHandler(FinanceDbContext db, IRepository<DebitOrigin, Guid> bankRepository)
        : base(db)
    {
    }

    public override async Task<ICollection<DebitOrigin?>> Handle(GetAllDebitOriginsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.DebitOrigin
            .Include(o => o.AppModule)
            .Include(o => o.Debits)
            .OrderBy(o => o.Name)
                .ThenBy(o => o.AppModule.Name)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return await Task.FromResult(await query.ToArrayAsync());
    }
}

public class GetAllDebitOriginsQuery : GetAllQuery<DebitOrigin?>
{
}
