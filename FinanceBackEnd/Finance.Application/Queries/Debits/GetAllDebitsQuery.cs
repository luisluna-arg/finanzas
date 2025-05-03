using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Debits;

public class GetAllDebitsQueryHandler : BaseCollectionHandler<GetAllDebitsQuery, Debit?>
{
    public GetAllDebitsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<Debit?>> Handle(GetAllDebitsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Debit.Include(o => o.Origin).ThenInclude(o => o.AppModule).AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.AppModuleId.HasValue)
        {
            query = query.Where(o => o.Origin.AppModuleId == request.AppModuleId);
        }

        return await query.OrderByDescending(o => o.TimeStamp).ThenBy(o => o.Origin.Name).ToArrayAsync();
    }
}

public class GetAllDebitsQuery : GetAllQuery<Debit?>
{
    public Guid? AppModuleId { get; set; }

    public FrequencyEnum Frequency { get; set; }
}
