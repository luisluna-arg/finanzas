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
        var query = DbContext.Debit.Include(o => o.DebitOrigin).ThenInclude(o => o.AppModule).AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (!string.IsNullOrWhiteSpace(request.AppModuleId))
        {
            query = query.Where(o => o.DebitOrigin.AppModuleId == new Guid(request.AppModuleId));
        }

        return await query.OrderByDescending(o => o.TimeStamp).ThenBy(o => o.DebitOrigin.Name).ToArrayAsync();
    }
}

public class GetAllDebitsQuery : GetAllQuery<Debit?>
{
    public string? AppModuleId { get; set; }
}
