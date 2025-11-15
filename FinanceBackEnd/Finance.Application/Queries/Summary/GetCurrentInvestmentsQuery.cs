using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Summary;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetCurrentInvestmentsQuery : IQuery<TotalInvestments>;

public class GetCurrentInvestmentsQueryHandler(FinanceDbContext db) : IQueryHandler<GetCurrentInvestmentsQuery, TotalInvestments>
{
    private readonly FinanceDbContext _db = db;

    public async Task<DataResult<TotalInvestments>> ExecuteAsync(GetCurrentInvestmentsQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalInvestments();

        var maxTimeStamp = await _db.IOLInvestment.MaxAsync(o => (DateTime?)o.TimeStamp, cancellationToken);
        if (maxTimeStamp == null)
            return DataResult<TotalInvestments>.Success(result);

        var investments = await _db.IOLInvestment
            .Include(o => o.Asset)
            .ThenInclude(o => o.Type)
            .Where(o => !o.Deactivated && o.TimeStamp == maxTimeStamp.Value)
            .OrderBy(o => o.Asset.Symbol)
            .ToArrayAsync(cancellationToken);

        result.Items.AddRange(investments.Select(o => new Investment()
        {
            Id = $"{o.Id}",
            Label = o.Asset.Symbol,
            Valued = o.Valued,
            AverageReturn = o.AverageReturn
        }));

        return DataResult<TotalInvestments>.Success(result);
    }
}
