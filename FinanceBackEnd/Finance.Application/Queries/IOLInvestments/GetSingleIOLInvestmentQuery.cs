using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.IOLInvestments;

public class GetSingleIOLInvestmentQuery : GetSingleByIdQuery<IOLInvestment?, Guid>;

public class GetSingleIOLInvestmentQueryHandler(FinanceDbContext db, IRepository<IOLInvestment, Guid> repository)
    : BaseResponseHandler<GetSingleIOLInvestmentQuery, IOLInvestment?>(db)
{
    private readonly IRepository<IOLInvestment, Guid> _repository = repository;

    public override async Task<IOLInvestment?> Handle(GetSingleIOLInvestmentQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id, cancellationToken);
}
