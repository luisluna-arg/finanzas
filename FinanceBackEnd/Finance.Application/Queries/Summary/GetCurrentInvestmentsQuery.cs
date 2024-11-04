using Finance.Application.Dtos.Summary;
using Finance.Domain;
using MediatR;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetCurrentInvestmentsQueryHandler : IRequestHandler<GetCurrentInvestmentsQuery, TotalInvestments>
{
    private readonly FinanceDbContext db;

    public GetCurrentInvestmentsQueryHandler(
        FinanceDbContext db)
    {
        this.db = db;
    }

    public Task<TotalInvestments> Handle(GetCurrentInvestmentsQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalInvestments();

        var maxTimeStamp = db.IOLInvestment.Max(o => o.TimeStamp);

        var investments = db.IOLInvestment
            .Include(o => o.Asset)
            .ThenInclude(o => o.Type)
            .Where(o => !o.Deactivated && o.TimeStamp == maxTimeStamp)
            .OrderBy(o => o.Asset.Symbol)
            .ToArray();

        result.Items.AddRange(investments.Select(o => new Investment($"{o.Id}", o.Asset.Symbol, o.AverageReturn, o.Valued)));

        return Task.FromResult(result);
    }
}

public class GetCurrentInvestmentsQuery : IRequest<TotalInvestments>
{
}
