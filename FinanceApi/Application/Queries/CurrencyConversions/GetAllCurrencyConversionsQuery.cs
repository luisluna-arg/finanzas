using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.CurrencyConvertions;

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

public class GetAllCurrencyConversionsQuery : GetAllQuery<CurrencyConversion>
{
}
