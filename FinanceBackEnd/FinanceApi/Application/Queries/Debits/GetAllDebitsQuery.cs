using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Debits;

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
