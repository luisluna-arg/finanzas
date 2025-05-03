using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Movements;

public class GetSingleMovementQueryHandler : BaseResponseHandler<GetSingleMovementQuery, Movement?>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    public GetSingleMovementQueryHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<Movement?> Handle(GetSingleMovementQuery request, CancellationToken cancellationToken)
        => await movementRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetSingleMovementQuery : GetSingleByIdQuery<Movement?, Guid>
{
}
