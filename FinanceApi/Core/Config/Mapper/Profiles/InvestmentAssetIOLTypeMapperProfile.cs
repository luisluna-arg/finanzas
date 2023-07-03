using AutoMapper;
using FinanceApi.Application.Dtos.InvestmentAssetIOLTypes;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class InvestmentAssetIOLTypeTypeMapperProfile : Profile
{
    public InvestmentAssetIOLTypeTypeMapperProfile() => CreateMap<InvestmentAssetIOLType, InvestmentAssetIOLTypeDto>();
}
