using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Movements;

public class GetAllMovementsQueryHandler : BaseCollectionHandler<GetAllMovementsQuery, Movement>
{
    public GetAllMovementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<Movement>> Handle(GetAllMovementsQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await DbContext.Movement.ToArrayAsync());
    }
}
