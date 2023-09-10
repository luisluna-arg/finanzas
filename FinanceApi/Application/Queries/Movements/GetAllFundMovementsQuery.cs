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
        => await Task.FromResult(await DbContext.Movement.Include(o => o.AppModule).Where(o => o.AppModule.Name == "Fondos").ToArrayAsync());
}

public class GetAllFundMovementsQuery : GetAllQuery<Movement>
{
}
