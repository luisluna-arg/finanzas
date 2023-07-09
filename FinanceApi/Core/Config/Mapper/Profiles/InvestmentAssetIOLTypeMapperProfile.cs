using AutoMapper;
using FinanceApi.Application.Dtos.InvestmentAssetIOLTypes;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class InvestmentAssetIOLTypeMapperProfile : Profile
{
    public InvestmentAssetIOLTypeMapperProfile() => CreateMap<InvestmentAssetIOLType, InvestmentAssetIOLTypeDto>();
}
