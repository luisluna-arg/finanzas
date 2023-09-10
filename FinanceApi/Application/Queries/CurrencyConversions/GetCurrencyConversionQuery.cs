using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Queries.CurrencyConvertions;

public class GetCurrencyConversionQueryHandler : BaseResponseHandler<GetCurrencyConversionQuery, CurrencyConversion?>
{
    private readonly IRepository<CurrencyConversion, Guid> currencyRepository;

    public GetCurrencyConversionQueryHandler(
        FinanceDbContext db,
        IRepository<CurrencyConversion, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<CurrencyConversion?> Handle(GetCurrencyConversionQuery request, CancellationToken cancellationToken)
        => await currencyRepository.GetById(request.Id);
}

public class GetCurrencyConversionQuery : GetSingleByIdQuery<CurrencyConversion, Guid>
{
}
