using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Queries.Movements;

public class GetMovementQueryHandler : BaseResponseHandler<GetMovementQuery, Movement?>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    public GetMovementQueryHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<Movement?> Handle(GetMovementQuery request, CancellationToken cancellationToken)
        => await movementRepository.GetById(request.Id);
}

public class GetMovementQuery : GetSingleByIdQuery<Movement, Guid>
{
}
