using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.Movements;

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
