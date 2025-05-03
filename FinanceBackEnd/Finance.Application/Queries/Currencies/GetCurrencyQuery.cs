using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Currencies;

public class GetCurrencyQueryHandler : BaseResponseHandler<GetCurrencyQuery, Currency?>
{
    private readonly IRepository<Currency, Guid> currencyRepository;

    public GetCurrencyQueryHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<Currency?> Handle(GetCurrencyQuery request, CancellationToken cancellationToken)
        => await currencyRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetCurrencyQuery : GetSingleByIdQuery<Currency?, Guid>
{
}
