using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.Movements;

public class GetAllMovementsQueryHandler : BaseCollectionHandler<GetAllMovementsQuery, Movement>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    public GetAllMovementsQueryHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<ICollection<Movement>> Handle(GetAllMovementsQuery request, CancellationToken cancellationToken)
        => await movementRepository.GetAll();
}

public class GetAllMovementsQuery : GetAllQuery<Movement>
{
}
