using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Movements;

public class GetSingleMovementQuery : GetSingleByIdQuery<Movement?, Guid>;

public class GetSingleMovementQueryHandler(FinanceDbContext db, IRepository<Movement, Guid> repository)
    : BaseResponseHandler<GetSingleMovementQuery, Movement?>(db)
{
    private readonly IRepository<Movement, Guid> _repository = repository;

    public override async Task<Movement?> Handle(GetSingleMovementQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id, cancellationToken);
}
