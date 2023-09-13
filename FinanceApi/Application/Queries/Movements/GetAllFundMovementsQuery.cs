using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Movements;

public class GetAllFundMovementsQueryHandler : BaseCollectionHandler<GetAllFundMovementsQuery, Movement>
{
    public GetAllFundMovementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<Movement>> Handle(GetAllFundMovementsQuery request, CancellationToken cancellationToken)
    {
        var q = DbContext.Movement.Include(o => o.AppModule).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.AppModuleId))
        {
            q = q.Where(o => o.AppModuleId == new Guid(request.AppModuleId));
        }

        return await q.ToArrayAsync();
    }
}

public class GetAllFundMovementsQuery : GetAllQuery<Movement>
{
    public GetAllFundMovementsQuery(string? appModuleId)
    {
        this.AppModuleId = appModuleId;
    }

    public string? AppModuleId { get; private set; }
}
