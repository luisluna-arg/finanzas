using Finance.Application.Dtos;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class PaginatedCurrencyExchangeRateMapperProfile : PaginatedResultMapperProfile<CurrencyExchangeRate, CurrencyExchangeRateDto>
{
    public PaginatedCurrencyExchangeRateMapperProfile()
        : base()
    {
    }
}