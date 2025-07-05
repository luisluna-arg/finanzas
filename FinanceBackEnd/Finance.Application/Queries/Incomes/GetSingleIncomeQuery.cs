using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Incomes;

public class GetSingleIncomeQuery : GetSingleByIdQuery<Income?, Guid>;

public class GetSingleIncomeQueryHandler(FinanceDbContext db, IRepository<Income, Guid> repository)
    : BaseResponseHandler<GetSingleIncomeQuery, Income?>(db)
{
    private readonly IRepository<Income, Guid> _repository = repository;

    public override async Task<Income?> Handle(GetSingleIncomeQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id, cancellationToken);
}
