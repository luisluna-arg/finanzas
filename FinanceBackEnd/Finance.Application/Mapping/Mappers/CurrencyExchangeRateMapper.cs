using Finance.Application.Dtos;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Mapping.Mappers;

public class CurrencyExchangeRateMapper : BaseMapper<CurrencyExchangeRate, CurrencyExchangeRateDto>, ICurrencyExchangeRateMapper
{
    public CurrencyExchangeRateMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICurrencyExchangeRateMapper : IMapper<CurrencyExchangeRate, CurrencyExchangeRateDto>;
