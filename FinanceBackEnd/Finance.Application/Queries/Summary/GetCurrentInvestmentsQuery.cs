using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Summary;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetCurrentInvestmentsQuery : IQuery<TotalInvestments>;

public class GetCurrentInvestmentsQueryHandler(FinanceDbContext db) : IQueryHandler<GetCurrentInvestmentsQuery, TotalInvestments>
{
    private readonly FinanceDbContext _db = db;

    public async Task<DataResult<TotalInvestments>> ExecuteAsync(GetCurrentInvestmentsQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalInvestments();

        var maxTimeStamp = _db.IOLInvestment.Max(o => o.TimeStamp);

        var investments = await _db.IOLInvestment
            .Include(o => o.Asset)
            .ThenInclude(o => o.Type)
            .Where(o => !o.Deactivated && o.TimeStamp == maxTimeStamp)
            .OrderBy(o => o.Asset.Symbol)
            .ToArrayAsync(cancellationToken);

        result.Items.AddRange(investments.Select(o => new Investment()
        {
            Id = $"{o.Id}",
            Label = o.Asset.Symbol,
            Value = o.Valued,
            AverageReturn = o.AverageReturn
        }));

        return DataResult<TotalInvestments>.Success(result);
    }
}
