using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Queries.Currencies;

public class GetCurrencyQueryHandler : BaseResponseHandler<GetCurrencyQuery, Currency>
{
    private readonly IRepository<Currency, Guid> currencyRepository;

    public GetCurrencyQueryHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<Currency> Handle(GetCurrencyQuery request, CancellationToken cancellationToken)
        => await currencyRepository.GetById(request.Id);
}

public class GetCurrencyQuery : GetSingleByIdQuery<Currency, Guid>
{
}
