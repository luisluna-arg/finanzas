using Finance.Application.Dtos.CurrencyConversions;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class CurrencyConversionMapperProfile : BaseEntityMapperProfile<CurrencyConversion, CurrencyConversionDto>
{
    public CurrencyConversionMapperProfile()
        : base()
    {
    }
}
