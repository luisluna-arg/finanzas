using Finance.Application.Legacy.Dtos;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class PaginatedCurrencyExchangeRateMapper : PaginatedResultMapper<CurrencyExchangeRate, CurrencyExchangeRateDto>, IPaginatedCurrencyExchangeRateMapper
{
    public PaginatedCurrencyExchangeRateMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedCurrencyExchangeRateMapper : IPaginatedResultMapper<CurrencyExchangeRate, CurrencyExchangeRateDto>;
