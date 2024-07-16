using FinanceApi.Application.Dtos.Currencies;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class CurrencyMapperProfile : BaseEntityMapperProfile<Currency, CurrencyDto>
{
    public CurrencyMapperProfile()
        : base()
    {
    }
}
