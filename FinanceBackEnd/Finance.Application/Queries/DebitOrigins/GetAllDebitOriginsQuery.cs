using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.DebitOrigins;

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
