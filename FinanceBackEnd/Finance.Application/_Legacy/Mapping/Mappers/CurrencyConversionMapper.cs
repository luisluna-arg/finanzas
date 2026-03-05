using Finance.Application.Legacy.Dtos.CurrencyConversions;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class CurrencyConversionMapper : BaseMapper<CurrencyConversion, CurrencyConversionDto>, ICurrencyConversionMapper
{
    public CurrencyConversionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICurrencyConversionMapper : IMapper<CurrencyConversion, CurrencyConversionDto>;
