using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Frequencies;

public class GetAllFrequenciesQueryHandler : BaseCollectionHandler<GetAllFrequenciesQuery, Frequency?>
{
    public GetAllFrequenciesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<Frequency?>> Handle(GetAllFrequenciesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Frequency
            .OrderBy(o => o.Name)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return await Task.FromResult(await query.ToArrayAsync());
    }
}

public class GetAllFrequenciesQuery : GetAllQuery<Frequency?>
{
}
