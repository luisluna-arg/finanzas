using Finance.Application.Dtos.Currencies;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Mapping.Mappers;

public class CurrencyMapper : BaseMapper<Currency, CurrencyDto>, ICurrencyMapper
{
    public CurrencyMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICurrencyMapper : IMapper<Currency, CurrencyDto>;

public class CurrencySymbolMapper : BaseMapper<CurrencySymbol, CurrencySymbolDto>, ICurrencySymbolMapper
{
    public CurrencySymbolMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICurrencySymbolMapper : IMapper<CurrencySymbol, CurrencySymbolDto>;
