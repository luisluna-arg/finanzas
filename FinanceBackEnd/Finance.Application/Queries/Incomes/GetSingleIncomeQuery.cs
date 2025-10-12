using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Queries.Incomes;

public class GetSingleIncomeQuery : GetSingleByIdQuery<Income?, Guid>;

public class GetSingleIncomeQueryHandler(FinanceDbContext db, IRepository<Income, Guid> repository)
    : BaseQueryHandler<GetSingleIncomeQuery, Income?>(db)
{
    private readonly IRepository<Income, Guid> _repository = repository;

    public override async Task<DataResult<Income?>> ExecuteAsync(GetSingleIncomeQuery request, CancellationToken cancellationToken)
        => DataResult<Income?>.Success(await _repository.GetByIdAsync(request.Id, cancellationToken));
}
