using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Currencies;

public class GetCurrencyQuery : GetSingleByIdQuery<Currency?, Guid>;

public class GetCurrencyQueryHandler(FinanceDbContext db, IRepository<Currency, Guid> repository)
    : BaseResponseHandler<GetCurrencyQuery, Currency?>(db)
{
    private readonly IRepository<Currency, Guid> _repository = repository;

    public override async Task<Currency?> Handle(GetCurrencyQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id, cancellationToken);
}
