using FinanceApi.Application.Dtos.CurrencyConversions;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class CurrencyConversionMapperProfile : BaseMapperProfile<CurrencyConversion, CurrencyConversionDto>
{
    public CurrencyConversionMapperProfile()
        : base()
    {
    }
}
