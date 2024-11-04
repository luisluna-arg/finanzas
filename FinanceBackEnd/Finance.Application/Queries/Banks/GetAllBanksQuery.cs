using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Banks;

public class GetAllBanksQueryHandler : BaseCollectionHandler<GetAllBanksQuery, Bank?>
{
    public GetAllBanksQueryHandler(FinanceDbContext db, IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
    }

    public override async Task<ICollection<Bank?>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Bank.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return await Task.FromResult(await query.OrderBy(o => o.Name).ToArrayAsync());
    }
}

public class GetAllBanksQuery : GetAllQuery<Bank?>
{
}
