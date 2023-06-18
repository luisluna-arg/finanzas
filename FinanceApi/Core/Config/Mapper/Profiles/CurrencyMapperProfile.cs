using AutoMapper;
using FinanceApi.Application.Dtos.Currency;
using FinanceApi.Application.Models;

namespace FinanceApi.Core.Config.Mapper
{
    public class CurrencyMapperProfile : Profile
    {
        public CurrencyMapperProfile() => CreateMap<Currency, CurrencyDto>();
    }
}
