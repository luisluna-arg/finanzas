using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Queries.Movements;

public class GetSingleMovementQuery : GetSingleByIdQuery<Movement?, Guid>;

public class GetSingleMovementQueryHandler(FinanceDbContext db, IRepository<Movement, Guid> repository)
    : BaseQueryHandler<GetSingleMovementQuery, Movement?>(db)
{
    private readonly IRepository<Movement, Guid> _repository = repository;

    public override async Task<DataResult<Movement?>> ExecuteAsync(GetSingleMovementQuery command, CancellationToken cancellationToken = default)
        => DataResult<Movement?>.Success(await _repository.GetByIdAsync(command.Id, cancellationToken));
}
