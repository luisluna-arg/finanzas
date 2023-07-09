using FinanceApi.Application.Queries.CurrencyConversions;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.CurrencyConvertions;

public class GetAllCurrencyConversionsQueryHandler : BaseCollectionHandler<GetAllCurrencyConversionsQuery, CurrencyConversion>
{
    private readonly IRepository<CurrencyConversion, Guid> currencyConversionRepository;

    public GetAllCurrencyConversionsQueryHandler(
        FinanceDbContext db,
        IRepository<CurrencyConversion, Guid> currencyConversionRepository)
        : base(db)
    {
        this.currencyConversionRepository = currencyConversionRepository;
    }

    public override async Task<ICollection<CurrencyConversion>> Handle(GetAllCurrencyConversionsQuery request, CancellationToken cancellationToken)
        => await currencyConversionRepository.GetAll();
}
