using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Queries.IOLInvestments;

public class GetSingleIOLInvestmentQuery : GetSingleByIdQuery<IOLInvestment?, Guid>;

public class GetSingleIOLInvestmentQueryHandler(FinanceDbContext db, IRepository<IOLInvestment, Guid> repository)
    : BaseQueryHandler<GetSingleIOLInvestmentQuery, IOLInvestment?>(db)
{
    private readonly IRepository<IOLInvestment, Guid> _repository = repository;

    public override async Task<DataResult<IOLInvestment?>> ExecuteAsync(GetSingleIOLInvestmentQuery request, CancellationToken cancellationToken)
        => DataResult<IOLInvestment?>.Success(await _repository.GetByIdAsync(request.Id, cancellationToken));
}
