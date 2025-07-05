using Finance.Application.Dtos.Summary;
using MediatR;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetCurrentInvestmentsQuery : IRequest<TotalInvestments>;

public class GetCurrentInvestmentsQueryHandler(FinanceDbContext db) : IRequestHandler<GetCurrentInvestmentsQuery, TotalInvestments>
{
    private readonly FinanceDbContext _db = db;

    public Task<TotalInvestments> Handle(GetCurrentInvestmentsQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalInvestments();

        var maxTimeStamp = _db.IOLInvestment.Max(o => o.TimeStamp);

        var investments = _db.IOLInvestment
            .Include(o => o.Asset)
            .ThenInclude(o => o.Type)
            .Where(o => !o.Deactivated && o.TimeStamp == maxTimeStamp)
            .OrderBy(o => o.Asset.Symbol)
            .ToArray();

        result.Items.AddRange(investments.Select(o => new Investment()
        {
            Id = $"{o.Id}",
            Label = o.Asset.Symbol,
            Value = o.Valued,
            AverageReturn = o.AverageReturn
        }));

        return Task.FromResult(result);
    }
}
