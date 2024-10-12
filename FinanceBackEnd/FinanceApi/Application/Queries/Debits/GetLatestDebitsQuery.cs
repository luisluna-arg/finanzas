using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Debits;

public class GetLatestDebitsQueryHandler : BaseCollectionHandler<GetLatestDebitsQuery, Debit?>
{
    public GetLatestDebitsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<Debit?>> Handle(GetLatestDebitsQuery request, CancellationToken cancellationToken)
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
                        .FirstOrDefaultAsync();

            if (record != null) debits.Add(record);
        }

        return debits.OrderBy(x => x.Origin.Name).ToArray();
    }
}

public class GetLatestDebitsQuery : GetAllQuery<Debit?>
{
    public Guid? AppModuleId { get; set; }

    public FrequencyEnum Frequency { get; set; }
}
