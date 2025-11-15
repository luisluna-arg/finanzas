using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Frequencies;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Frequencies;

public class GetAllFrequenciesQuery : GetAllQuery<Frequency>;

public class GetAllFrequenciesQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetAllFrequenciesQuery, Frequency>(db)
{
    public override async Task<DataResult<List<Frequency>>> ExecuteAsync(GetAllFrequenciesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Frequency
            .OrderBy(o => o.Name)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<Frequency>>.Success(await query.ToListAsync(cancellationToken));
    }
}
