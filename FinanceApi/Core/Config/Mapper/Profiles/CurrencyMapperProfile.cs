using AutoMapper;
using FinanceApi.Application.Dtos.Currencies;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class CurrencyMapperProfile : Profile
{
    public CurrencyMapperProfile() => CreateMap<Currency, CurrencyDto>();
}
