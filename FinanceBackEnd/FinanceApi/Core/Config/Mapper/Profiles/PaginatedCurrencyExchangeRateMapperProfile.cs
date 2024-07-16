using FinanceApi.Application.Dtos;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class PaginatedCurrencyExchangeRateMapperProfile : PaginatedResultMapperProfile<CurrencyExchangeRate, CurrencyExchangeRateDto>
{
    public PaginatedCurrencyExchangeRateMapperProfile()
        : base()
    {
    }
}