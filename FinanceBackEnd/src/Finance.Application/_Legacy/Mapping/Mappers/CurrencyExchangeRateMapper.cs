using Finance.Application.Legacy.Dtos;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class CurrencyExchangeRateMapper : BaseMapper<CurrencyExchangeRate, CurrencyExchangeRateDto>, ICurrencyExchangeRateMapper
{
    public CurrencyExchangeRateMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICurrencyExchangeRateMapper : IMapper<CurrencyExchangeRate, CurrencyExchangeRateDto>;
