using Finance.Application.Dtos;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Mapping.Mappers;

public class PaginatedCurrencyExchangeRateMapper : PaginatedResultMapper<CurrencyExchangeRate, CurrencyExchangeRateDto>, IPaginatedCurrencyExchangeRateMapper
{
    public PaginatedCurrencyExchangeRateMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedCurrencyExchangeRateMapper : IPaginatedResultMapper<CurrencyExchangeRate, CurrencyExchangeRateDto>;
