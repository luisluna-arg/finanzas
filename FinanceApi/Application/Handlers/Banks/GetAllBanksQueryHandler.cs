using FinanceApi.Application.Queries.Banks;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Banks;

public class GetAllBanksQueryHandler : BaseCollectionHandler<GetAllBanksQuery, Bank>
{
    public GetAllBanksQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<Bank>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await DbContext.Bank.ToArrayAsync());
    }
}
