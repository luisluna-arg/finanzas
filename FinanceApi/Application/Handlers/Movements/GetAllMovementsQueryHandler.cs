using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Movements;

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
