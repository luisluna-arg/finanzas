using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Banks;

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
