using Finance.Application.Legacy.Dtos.Currencies;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class CurrencyMapper : BaseMapper<Currency, CurrencyDto>, ICurrencyMapper
{
    public CurrencyMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICurrencyMapper : IMapper<Currency, CurrencyDto>;
