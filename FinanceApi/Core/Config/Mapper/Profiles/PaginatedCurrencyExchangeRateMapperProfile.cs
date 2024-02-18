using AutoMapper;
using FinanceApi.Application.Dtos;
using FinanceApi.Commons;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class PaginatedCurrencyExchangeRateMapperProfile : Profile
{
    public PaginatedCurrencyExchangeRateMapperProfile()
    {
        CreateMap<PaginatedResult<CurrencyExchangeRate>, PaginatedResult<CurrencyExchangeRateDto>>();
    }
}