using FinanceApi.Application.Dtos;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class CurrencyExchangeRateMapperProfile : BaseMapperProfile<CurrencyExchangeRate, CurrencyExchangeRateDto>
{
    public CurrencyExchangeRateMapperProfile()
        : base()
    {
    }
}
