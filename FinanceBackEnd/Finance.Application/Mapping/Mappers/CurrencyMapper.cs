using Finance.Application.Mapping.Base;
using Finance.Domain.Models;
using Finance.Application.Dtos.Currencies;

namespace Finance.Application.Mapping.Mappers;

public class CurrencyMapper : BaseMapper<Currency, CurrencyDto>, ICurrencyMapper
{
    public CurrencyMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICurrencyMapper : IMapper<Currency, CurrencyDto>
{
}
