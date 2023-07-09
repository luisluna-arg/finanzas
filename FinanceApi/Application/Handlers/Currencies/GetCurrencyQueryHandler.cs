using FinanceApi.Application.Queries.Currencies;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Currencies;

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
