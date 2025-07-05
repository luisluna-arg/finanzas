using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Funds;

public class GetSingleFundQuery : GetSingleByIdQuery<Fund?, Guid>;

public class GetSingleFundQueryHandler(FinanceDbContext db, IRepository<Fund, Guid> repository)
    : BaseResponseHandler<GetSingleFundQuery, Fund?>(db)
{
    private readonly IRepository<Fund, Guid> _repository = repository;

    public override async Task<Fund?> Handle(GetSingleFundQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id, cancellationToken);
}
