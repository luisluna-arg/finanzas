using AutoMapper;
using FinanceApi.Application.Dtos.CurrencyConversions;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class CurrencyConversionMapperProfile : Profile
{
    public CurrencyConversionMapperProfile() => CreateMap<CurrencyConversion, CurrencyConversionDto>();
}
