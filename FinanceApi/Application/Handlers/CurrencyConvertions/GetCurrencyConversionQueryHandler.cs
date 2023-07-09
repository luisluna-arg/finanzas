using FinanceApi.Application.Queries.CurrencyConversions;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.CurrencyConvertions;

public class GetCurrencyConversionQueryHandler : BaseResponseHandler<GetCurrencyConversionQuery, CurrencyConversion>
{
    private readonly IRepository<CurrencyConversion, Guid> currencyRepository;

    public GetCurrencyConversionQueryHandler(
        FinanceDbContext db,
        IRepository<CurrencyConversion, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<CurrencyConversion> Handle(GetCurrencyConversionQuery request, CancellationToken cancellationToken)
        => await currencyRepository.GetById(request.Id);
}
