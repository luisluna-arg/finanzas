using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Debits;

public class GetLatestDebitsQueryHandler : BaseCollectionHandler<GetLatestDebitsQuery, Debit>
{
    public GetLatestDebitsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<Debit>>> ExecuteAsync(GetLatestDebitsQuery request, CancellationToken cancellationToken)
    {
        var originIds = DbContext.DebitOrigin.Where(o => o.AppModuleId == request.AppModuleId).Select(o => o.Id).ToArray();

        var baseQuery = DbContext.Debit
                .Include(o => o.Origin)
                .ThenInclude(o => o.AppModule)
                .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            baseQuery = baseQuery.Where(o => !o.Origin.AppModule.Deactivated && !o.Origin.Deactivated && !o.Deactivated);
        }

        var debits = new List<Debit>();

        foreach (var id in originIds)
        {
            var record = await baseQuery
                        .Where(o => o.OriginId == id)
                        .OrderByDescending(e => e.TimeStamp)
                        .FirstOrDefaultAsync(cancellationToken);

            if (record != null) debits.Add(record);
        }

        return DataResult<List<Debit>>.Success(debits.OrderBy(x => x.Origin.Name).ToList());
    }
}

public class GetLatestDebitsQuery : GetAllQuery<Debit>
{
    public Guid? AppModuleId { get; set; }
    public FrequencyEnum Frequency { get; set; }
}
