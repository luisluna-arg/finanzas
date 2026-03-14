using Finance.Application.Dtos.CurrencyConversions;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Mapping.Mappers;

public class CurrencyConversionMapper : BaseMapper<CurrencyConversion, CurrencyConversionDto>, ICurrencyConversionMapper
{
    public CurrencyConversionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICurrencyConversionMapper : IMapper<CurrencyConversion, CurrencyConversionDto>;
