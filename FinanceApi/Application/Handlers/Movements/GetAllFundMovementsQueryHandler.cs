using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Movements;

public class GetAllFundMovementsQueryHandler : BaseCollectionHandler<GetAllFundMovementsQuery, Movement>
{
    public GetAllFundMovementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<Movement>> Handle(GetAllFundMovementsQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await DbContext.Movement.Include(o => o.AppModule).Where(o => o.AppModule.Name == "Fondos").ToArrayAsync());
    }
}
